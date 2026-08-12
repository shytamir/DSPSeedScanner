using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Runtime
{
    public enum PreviewPanelLineKind
    {
        Identity,
        Status,
        Section,
        Context,
        Conclusion
    }

    public enum PreviewConclusionColumn
    {
        Strength,
        PreferenceSensitive,
        Limitation
    }

    public sealed record PreviewPanelLine
    {
        public PreviewPanelLine(PreviewPanelLineKind kind, string text)
        {
            if (String.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Panel line text is required.", nameof(text));
            if (text.Length > PreviewConclusionPresenter.MaximumLineCharacters)
                throw new ArgumentException("Panel line exceeds its presentation bound.", nameof(text));
            Kind = kind;
            Text = text;
        }

        public PreviewPanelLineKind Kind { get; }
        public string Text { get; }
    }

    public sealed record PresentedConclusionCard
    {
        internal PresentedConclusionCard(
            ConclusionContext context,
            EvidenceStage stage,
            ComponentOutcome outcome,
            string family,
            string title,
            IEnumerable<string> subjects,
            string line,
            IEnumerable<string> sourceConclusionIds)
        {
            Context = context;
            Stage = stage;
            Outcome = outcome;
            Column = ColumnFor(outcome);
            Family = family;
            Title = title;
            Subjects = Array.AsReadOnly(subjects.ToArray());
            Line = line;
            SourceConclusionIds = Array.AsReadOnly(sourceConclusionIds.ToArray());
        }

        public ConclusionContext Context { get; }
        public EvidenceStage Stage { get; }
        public ComponentOutcome Outcome { get; }
        public PreviewConclusionColumn Column { get; }
        public string Family { get; }
        public string Title { get; }
        public IReadOnlyList<string> Subjects { get; }
        public string Line { get; }
        public IReadOnlyList<string> SourceConclusionIds { get; }

        private static PreviewConclusionColumn ColumnFor(ComponentOutcome outcome) =>
            outcome switch
            {
                ComponentOutcome.Supports => PreviewConclusionColumn.Strength,
                ComponentOutcome.PreferenceSensitive or ComponentOutcome.Tradeoff =>
                    PreviewConclusionColumn.PreferenceSensitive,
                ComponentOutcome.DoesNotSupport or ComponentOutcome.Caution =>
                    PreviewConclusionColumn.Limitation,
                _ => throw new ArgumentOutOfRangeException(nameof(outcome))
            };
    }

    public sealed record PresentedContextGroup
    {
        internal PresentedContextGroup(
            ConclusionContext context,
            string title,
            IEnumerable<PresentedConclusionCard> cards)
        {
            Context = context;
            Title = title;
            Cards = Array.AsReadOnly(cards.ToArray());
        }

        public ConclusionContext Context { get; }
        public string Title { get; }
        public IReadOnlyList<PresentedConclusionCard> Cards { get; }
    }

    public sealed record PreviewConclusionPresentation
    {
        internal PreviewConclusionPresentation(
            long sessionId,
            string identityLine,
            string? darkFogStatusLine,
            bool isCached,
            IEnumerable<PresentedContextGroup> immediateGroups,
            IEnumerable<PresentedContextGroup> detailGroups)
        {
            SessionId = sessionId;
            IdentityLine = identityLine;
            DarkFogStatusLine = darkFogStatusLine;
            IsCached = isCached;
            ImmediateGroups = Array.AsReadOnly(immediateGroups.ToArray());
            DetailGroups = Array.AsReadOnly(detailGroups.ToArray());
        }

        public long SessionId { get; }
        public string IdentityLine { get; }
        public string? DarkFogStatusLine { get; }
        public bool IsCached { get; }
        public IReadOnlyList<PresentedContextGroup> ImmediateGroups { get; }
        public IReadOnlyList<PresentedContextGroup> DetailGroups { get; }
    }

    public sealed record PreviewPanelDocument
    {
        internal PreviewPanelDocument(IEnumerable<PreviewPanelLine> lines)
        {
            PreviewPanelLine[] values = lines.ToArray();
            if (values.Length == 0 || values.Length > PreviewConclusionPresenter.MaximumDocumentLines)
                throw new ArgumentException("Panel document line count is outside its bound.", nameof(lines));
            Lines = Array.AsReadOnly(values);
        }

        public IReadOnlyList<PreviewPanelLine> Lines { get; }
    }

    public static class PreviewConclusionPresenter
    {
        public const int MaximumLineCharacters = 240;
        public const int MaximumDocumentLines = 72;
        public const int MaximumSubjectsPerCard = 3;

        private static readonly ConclusionContext[] ContextOrder =
        {
            ConclusionContext.FreshStart,
            ConclusionContext.Megafactory,
            ConclusionContext.CompactExpansion,
            ConclusionContext.SphereShowcase,
            ConclusionContext.DecisionRelevantTraits
        };

        public static PreviewConclusionPresentation Project(PreviewResolutionAttempt attempt)
        {
            if (attempt == null)
                throw new ArgumentNullException(nameof(attempt));

            PreviewGenerationIdentity identity = attempt.Session.Identity;
            IReadOnlyDictionary<string, RuntimeSystemDisplay> displays =
                attempt.SystemDisplays.ToDictionary(
                    value => value.Identifier,
                    StringComparer.Ordinal);
            ConclusionReport[] completedCompact = attempt.CompleteReports.Where(report =>
                report.Context == ConclusionContext.CompactExpansion).ToArray();
            IEnumerable<ConclusionReport> immediateReports = attempt.PreviewReports.Where(report =>
                report.Stage == EvidenceStage.GalaxyPreview &&
                (completedCompact.Length == 0 ||
                    report.Context != ConclusionContext.CompactExpansion));
            IEnumerable<ConclusionReport> detailReports = attempt.CompleteReports.Where(report =>
                report.Stage == EvidenceStage.BirthSystemRaw ||
                report.Stage == EvidenceStage.CompleteClusterRaw);
            if (completedCompact.Length != 0)
            {
                detailReports = detailReports.Concat(attempt.PreviewReports.Where(report =>
                    report.Context == ConclusionContext.CompactExpansion &&
                    report.Stage == EvidenceStage.GalaxyPreview));
            }
            return new PreviewConclusionPresentation(
                attempt.Session.SessionId,
                IdentityLine(identity),
                DarkFogStatusLine(attempt.DarkFogOccupation),
                attempt.State == PreviewResolutionState.Cached,
                Group(
                    immediateReports,
                    displays,
                    attempt.SystemCandidates,
                    attempt.HasCompleteBirthPlanetAttribution
                        ? attempt.BirthPlanetAttributions
                        : null),
                Group(
                    detailReports,
                    displays,
                    attempt.SystemCandidates,
                    attempt.HasCompleteBirthPlanetAttribution
                        ? attempt.BirthPlanetAttributions
                        : null));
        }

        public static PresentedConclusionCard MapCard(ConclusionReport report)
        {
            return MapCard(report, Array.Empty<RuntimeSystemDisplay>());
        }

        public static PresentedConclusionCard MapCard(
            ConclusionReport report,
            IEnumerable<RuntimeSystemDisplay> systemDisplays)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));
            if (systemDisplays == null)
                throw new ArgumentNullException(nameof(systemDisplays));
            return BuildCard(
                new[] { report },
                systemDisplays.ToDictionary(
                    value => value.Identifier,
                    StringComparer.Ordinal)) ??
                throw new InvalidOperationException(
                    "The conclusion has no player-readable presentation.");
        }

        public static PreviewPanelDocument Compose(
            PreviewPanelView operational,
            PreviewConclusionPresentation? conclusions)
        {
            if (operational == null)
                throw new ArgumentNullException(nameof(operational));
            if (!operational.Visible)
                throw new ArgumentException("A hidden panel has no document.", nameof(operational));

            var lines = new List<PreviewPanelLine>();
            if (conclusions != null)
            {
                if (conclusions.SessionId != operational.SessionId)
                    throw new ArgumentException("Panel conclusions belong to another session.", nameof(conclusions));
                lines.Add(new PreviewPanelLine(
                    PreviewPanelLineKind.Identity,
                    conclusions.IdentityLine));
            }

            string status = operational.Spinner.HasValue
                ? operational.Spinner.Value + "  " + operational.Title + " - " + operational.Detail
                : operational.Title + " - " + operational.Detail;
            lines.Add(new PreviewPanelLine(PreviewPanelLineKind.Status, status));
            if (conclusions == null)
                return new PreviewPanelDocument(lines);

            if (conclusions.DarkFogStatusLine != null)
            {
                lines.Add(new PreviewPanelLine(
                    PreviewPanelLineKind.Status,
                    conclusions.DarkFogStatusLine));
            }

            AddGroups(lines, conclusions.ImmediateGroups);
            AddGroups(lines, conclusions.DetailGroups);
            return new PreviewPanelDocument(lines);
        }

        private static IReadOnlyList<PresentedContextGroup> Group(
            IEnumerable<ConclusionReport> source,
            IReadOnlyDictionary<string, RuntimeSystemDisplay> displays,
            RuntimeSystemCandidates? candidates,
            IReadOnlyList<NormalizedBirthPlanetEvidence>? birthPlanets)
        {
            ConclusionReport[] reports = source
                .Where(report => report.Outcome != ComponentOutcome.Unknown &&
                    report.Outcome != ComponentOutcome.NotApplicable)
                .ToArray();
            var groups = new List<PresentedContextGroup>();
            foreach (ConclusionContext context in ContextOrder)
            {
                ConclusionReport[] contextReports = reports
                    .Where(report => report.Context == context)
                    .ToArray();
                if (contextReports.Length == 0)
                    continue;

                if (context == ConclusionContext.FreshStart)
                {
                    IReadOnlyList<PresentedConclusionCard> freshStart =
                        BuildFreshStartCards(contextReports, birthPlanets);
                    if (freshStart.Count != 0)
                    {
                        groups.Add(new PresentedContextGroup(
                            context,
                            ContextTitle(context),
                            freshStart));
                    }
                    continue;
                }

                if (context == ConclusionContext.Megafactory)
                {
                    IReadOnlyList<PresentedConclusionCard> megafactory =
                        BuildMegafactoryCards(reports, candidates, displays);
                    if (megafactory.Count != 0)
                    {
                        groups.Add(new PresentedContextGroup(
                            context,
                            ContextTitle(context),
                            megafactory));
                    }
                    continue;
                }

                if (context == ConclusionContext.CompactExpansion)
                {
                    IReadOnlyList<PresentedConclusionCard> compact =
                        BuildCompactExpansionCards(contextReports);
                    if (compact.Count != 0)
                    {
                        groups.Add(new PresentedContextGroup(
                            context,
                            ContextTitle(context),
                            compact));
                    }
                    continue;
                }

                var cards = new List<PresentedConclusionCard>();
                var ordinary = new Dictionary<string, List<ConclusionReport>>(StringComparer.Ordinal);
                var order = new List<string>();
                foreach (ConclusionReport report in contextReports)
                {
                    if (report.Outcome == ComponentOutcome.Tradeoff ||
                        report.Outcome == ComponentOutcome.Caution)
                    {
                        PresentedConclusionCard? exceptional = BuildCard(
                            new[] { report },
                            displays);
                        if (exceptional != null)
                            cards.Add(exceptional);
                        continue;
                    }

                    string family = Family(report.ConclusionId);
                    string key = family + "\t" + ((int)report.Outcome).ToString(
                        CultureInfo.InvariantCulture);
                    if (!ordinary.TryGetValue(key, out List<ConclusionReport>? bucket))
                    {
                        bucket = new List<ConclusionReport>();
                        ordinary.Add(key, bucket);
                        order.Add(key);
                    }
                    bucket.Add(report);
                }
                foreach (string key in order)
                {
                    PresentedConclusionCard? card = BuildCard(
                        ordinary[key],
                        displays);
                    if (card != null)
                        cards.Add(card);
                }

                if (cards.Count != 0)
                    groups.Add(new PresentedContextGroup(context, ContextTitle(context), cards));
            }
            return Array.AsReadOnly(groups.ToArray());
        }

        private static PresentedConclusionCard? BuildCard(
            IReadOnlyList<ConclusionReport> reports,
            IReadOnlyDictionary<string, RuntimeSystemDisplay> displays)
        {
            if (reports.Count == 0)
                throw new ArgumentException("A presentation card requires a source report.", nameof(reports));
            ConclusionReport first = reports[0];
            string family = Family(first.ConclusionId);
            if (reports.Any(report =>
                report.Context != first.Context ||
                report.Stage != first.Stage ||
                report.Outcome != first.Outcome ||
                !String.Equals(Family(report.ConclusionId), family, StringComparison.Ordinal)))
            {
                throw new ArgumentException("A presentation card cannot merge unlike conclusions.", nameof(reports));
            }

            string[] subjects = reports
                .Select(report => SubjectLabel(report, displays))
                .Where(value => value != null)
                .Select(value => value!)
                .Distinct(StringComparer.Ordinal)
                .ToArray();
            if (subjects.Length == 0)
                return null;
            string title = FamilyTitle(family, first);
            string line = Bound(
                title + " - " + SubjectSummary(subjects),
                MaximumLineCharacters);
            return new PresentedConclusionCard(
                first.Context,
                first.Stage,
                first.Outcome,
                family,
                title,
                subjects,
                line,
                reports.Select(report => report.ConclusionId));
        }

        private static IReadOnlyList<PresentedConclusionCard> BuildFreshStartCards(
            IReadOnlyList<ConclusionReport> reports,
            IReadOnlyList<NormalizedBirthPlanetEvidence>? birthPlanets)
        {
            var cards = new List<PresentedConclusionCard>();
            foreach (ComponentOutcome outcome in new[]
            {
                ComponentOutcome.Supports,
                ComponentOutcome.PreferenceSensitive,
                ComponentOutcome.DoesNotSupport
            })
            {
                ConclusionReport[] gasReports = reports.Where(report =>
                    report.ConclusionId.StartsWith(
                        "FS-GAS-ROUTE.product:",
                        StringComparison.Ordinal) &&
                    report.Outcome == outcome).ToArray();
                AddFreshCard(cards, gasReports, GasLine(gasReports, birthPlanets));
            }
            AddFreshCard(cards, Reports(reports, "FS-POWER.solar"), PowerLine(
                reports,
                birthPlanets,
                "FS-POWER.solar",
                "solar",
                "bright",
                "normal",
                "dim",
                planet => planet.SolarRatio));
            AddFreshCard(cards, Reports(reports, "FS-POWER.wind"), PowerLine(
                reports,
                birthPlanets,
                "FS-POWER.wind",
                "wind",
                "strong",
                "normal",
                "weak",
                planet => planet.WindRatio));
            AddFreshCard(cards, Reports(reports, "FS-POWER.birth-tidal"), TidalLine(
                reports,
                birthPlanets));
            AddFreshCard(
                cards,
                Reports(reports, SharedSatelliteEvaluator.ConclusionId),
                TopologyLine(reports));

            foreach (ConclusionReport report in reports.Where(report =>
                report.ConclusionId.StartsWith("FS-RESOURCES.amount:", StringComparison.Ordinal) ||
                report.ConclusionId.StartsWith("FS-RESOURCES.groups:", StringComparison.Ordinal) ||
                report.ConclusionId == "FS-RESOURCES.fire-ice"))
            {
                AddFreshCard(
                    cards,
                    new[] { report },
                    ResourceLine(report));
            }
            return Array.AsReadOnly(cards.ToArray());
        }

        private static IReadOnlyList<PresentedConclusionCard> BuildMegafactoryCards(
            IReadOnlyList<ConclusionReport> reports,
            RuntimeSystemCandidates? candidates,
            IReadOnlyDictionary<string, RuntimeSystemDisplay> displays)
        {
            var cards = new List<PresentedConclusionCard>();
            var roles = new Dictionary<string, MegaSystemCard>(StringComparer.Ordinal);

            ConclusionReport? output = SingleReport(reports, "MF-ENERGY-SYSTEM.output");
            ConclusionReport? separation = SingleReport(
                reports,
                "MF-ENERGY-SYSTEM.separation");
            if (output != null && candidates?.Energy != null)
            {
                RuntimeSystemCandidate[] bright = candidates.Energy.Where(candidate =>
                    ConclusionDefinition.Evaluate(
                        candidate.DecisiveValue,
                        ConclusionDefinition.EnergyOutput) == ComponentOutcome.Supports)
                    .ToArray();
                if (output.Outcome == ComponentOutcome.DoesNotSupport)
                {
                    AddMegaCard(cards, new[] { output }, "No bright stars");
                }
                else if (output.Outcome == ComponentOutcome.PreferenceSensitive)
                {
                    RuntimeSystemCandidate leader = candidates.Energy[0];
                    string description = separation?.Outcome == ComponentOutcome.Supports
                        ? "brightest"
                        : "bright";
                    AddMegaRole(roles, leader, output.Outcome, description, output,
                        separation);
                }
                else if (output.Outcome == ComponentOutcome.Supports)
                {
                    if (candidates.EnergySupportingCount > MaximumSubjectsPerCard)
                    {
                        AddMegaCard(
                            cards,
                            new[] { output },
                            "Many bright stars: " + JoinNames(
                                bright.Select(candidate => candidate.DisplayName)));
                    }
                    else
                    {
                        for (int index = 0; index < bright.Length; index++)
                        {
                            string description = index == 0
                                ? separation?.Outcome == ComponentOutcome.Supports
                                    ? "outshines all"
                                    : "unusually bright"
                                : "bright";
                            AddMegaRole(roles, bright[index], output.Outcome, description,
                                output, index == 0 ? separation : null);
                        }
                    }
                }
            }

            ConclusionReport? radius = SingleReport(
                reports,
                "MF-SPHERE-GEOMETRY.radius");
            if (radius != null && candidates?.ShellRadius != null)
            {
                RuntimeSystemCandidate[] large = candidates.ShellRadius.Where(candidate =>
                    ConclusionDefinition.Evaluate(
                        candidate.DecisiveValue,
                        ConclusionDefinition.SphereRadius) == ComponentOutcome.Supports)
                    .ToArray();
                if (radius.Outcome == ComponentOutcome.DoesNotSupport)
                {
                    AddMegaCard(cards, new[] { radius }, "No large spheres");
                }
                else if (radius.Outcome == ComponentOutcome.Supports)
                {
                    if (candidates.ShellRadiusSupportingCount > MaximumSubjectsPerCard)
                    {
                        AddMegaCard(
                            cards,
                            new[] { radius },
                            "Many large spheres: " + JoinNames(
                                large.Select(candidate => candidate.DisplayName)));
                    }
                    else
                    {
                        foreach (RuntimeSystemCandidate candidate in large)
                            AddMegaRole(roles, candidate, radius.Outcome, "large sphere", radius);
                    }
                }
            }

            ConclusionReport[] containmentReports = reports.Where(report =>
                report.ConclusionId == "MF-SPHERE-GEOMETRY.containment").ToArray();
            if (containmentReports.Length != 0 && candidates?.ContainedOrbits != null)
            {
                RuntimeSystemCandidate[] contained = candidates.ContainedOrbits.Where(candidate =>
                    ConclusionDefinition.Evaluate(
                        candidate.DecisiveValue,
                        ConclusionDefinition.OrbitContainment) == ComponentOutcome.Supports)
                    .ToArray();
                if (contained.Length == 0 && containmentReports.All(report =>
                    report.Outcome == ComponentOutcome.DoesNotSupport))
                {
                    AddMegaCard(cards, containmentReports, "No contained orbits");
                }
                else if (candidates.ContainedOrbitsSupportingCount > MaximumSubjectsPerCard)
                {
                    AddMegaCard(
                        cards,
                        containmentReports.Where(report =>
                            report.Outcome == ComponentOutcome.Supports).ToArray(),
                        "Many contained-orbit systems: " + JoinNames(
                            contained.Select(candidate => candidate.DisplayName)));
                }
                else
                {
                    foreach (RuntimeSystemCandidate candidate in contained)
                    {
                        ConclusionReport? report = containmentReports.SingleOrDefault(value =>
                            value.Subject.Identifier == candidate.Identifier &&
                            value.Outcome == ComponentOutcome.Supports);
                        if (report == null)
                            continue;
                        int count = Decimal.ToInt32(candidate.DecisiveValue);
                        AddMegaRole(
                            roles,
                            candidate,
                            ComponentOutcome.Supports,
                            count == 1 ? "contained orbit" :
                                count.ToString(CultureInfo.InvariantCulture) +
                                " contained orbits",
                            report);
                    }
                }
            }

            AddRareAccessCards(cards, roles, reports, displays);
            foreach (MegaSystemCard role in roles.Values
                .OrderBy(value => value.Outcome)
                .ThenBy(value => value.DisplayName, StringComparer.Ordinal))
            {
                string line = role.Descriptions.Count == 1
                    ? SingleMegaRoleLine(role.DisplayName, role.Descriptions[0])
                    : role.DisplayName + ": " + String.Join(", ", role.Descriptions);
                AddMegaCard(cards, role.Sources, line);
            }
            return Array.AsReadOnly(cards.ToArray());
        }

        private static IReadOnlyList<PresentedConclusionCard> BuildCompactExpansionCards(
            IReadOnlyList<ConclusionReport> reports)
        {
            var bestByRole = new Dictionary<string, CompactRoleRoute>(StringComparer.Ordinal);
            foreach (ConclusionReport report in reports.Where(report =>
                report.ConclusionId == "CX-GROUPING.distance" &&
                (report.Outcome == ComponentOutcome.Supports ||
                 report.Outcome == ComponentOutcome.PreferenceSensitive ||
                 report.Outcome == ComponentOutcome.DoesNotSupport)))
            {
                foreach (string role in CompactRoles(report.Subject.Identifier))
                {
                    var candidate = new CompactRoleRoute(role, report.Outcome, report);
                    if (!bestByRole.TryGetValue(role, out CompactRoleRoute? current) ||
                        RoutePriority(candidate.Outcome) < RoutePriority(current.Outcome))
                    {
                        bestByRole[role] = candidate;
                    }
                }
            }

            var cards = new List<PresentedConclusionCard>();
            foreach ((ComponentOutcome outcome, string title) in new[]
            {
                (ComponentOutcome.Supports, "Short routes"),
                (ComponentOutcome.PreferenceSensitive, "Normal routes"),
                (ComponentOutcome.DoesNotSupport, "Long routes")
            })
            {
                CompactRoleRoute[] routes = bestByRole.Values
                    .Where(value => value.Outcome == outcome)
                    .OrderBy(value => CompactRoleOrder(value.Role))
                    .ToArray();
                if (routes.Length == 0)
                    continue;
                string[] namedRoles = routes
                    .Take(MaximumSubjectsPerCard)
                    .Select(value => CompactRoleLabel(value.Role))
                    .ToArray();
                string line = title + (namedRoles.Length == 0
                    ? String.Empty
                    : ": " + String.Join(", ", namedRoles));
                EvidenceStage stage = routes.Max(value => value.Source.Stage);
                cards.Add(new PresentedConclusionCard(
                    ConclusionContext.CompactExpansion,
                    stage,
                    outcome,
                    "COMPACT-ROUTES",
                    title,
                    Array.Empty<string>(),
                    Bound(line, MaximumLineCharacters),
                    routes.Select(value => value.Source.ConclusionId).Distinct(
                        StringComparer.Ordinal)));
            }
            return Array.AsReadOnly(cards.ToArray());
        }

        private static IEnumerable<string> CompactRoles(string pairIdentifier)
        {
            int separator = pairIdentifier.LastIndexOf(':');
            if (separator < 0 || separator == pairIdentifier.Length - 1)
                yield break;
            foreach (string role in pairIdentifier.Substring(separator + 1).Split('+'))
            {
                if (CompactRoleOrder(role) != Int32.MaxValue)
                    yield return role;
            }
        }

        private static int RoutePriority(ComponentOutcome outcome) => outcome switch
        {
            ComponentOutcome.Supports => 0,
            ComponentOutcome.PreferenceSensitive => 1,
            ComponentOutcome.DoesNotSupport => 2,
            _ => Int32.MaxValue
        };

        private static int CompactRoleOrder(string role) => role switch
        {
            "starter-anchor" => 0,
            "strong-energy" => 1,
            "large-shell" => 2,
            "orbit-containment" => 3,
            "rare-access" => 4,
            _ => Int32.MaxValue
        };

        private static string CompactRoleLabel(string role) => role switch
        {
            "starter-anchor" => "starter",
            "strong-energy" => "energy",
            "large-shell" => "sphere",
            "orbit-containment" => "orbits",
            "rare-access" => "rares",
            _ => throw new ArgumentOutOfRangeException(nameof(role))
        };

        private sealed class CompactRoleRoute
        {
            public CompactRoleRoute(
                string role,
                ComponentOutcome outcome,
                ConclusionReport source)
            {
                Role = role;
                Outcome = outcome;
                Source = source;
            }

            public string Role { get; }
            public ComponentOutcome Outcome { get; }
            public ConclusionReport Source { get; }
        }

        private static void AddRareAccessCards(
            ICollection<PresentedConclusionCard> cards,
            IDictionary<string, MegaSystemCard> roles,
            IReadOnlyList<ConclusionReport> reports,
            IReadOnlyDictionary<string, RuntimeSystemDisplay> displays)
        {
            ConclusionReport[] rareReports = reports.Where(report =>
                report.ConclusionId.StartsWith("RR-ACCESS.distance:", StringComparison.Ordinal))
                .ToArray();
            foreach (IGrouping<(ComponentOutcome Outcome, bool IsAbsent), ConclusionReport>
                group in rareReports.GroupBy(report =>
                    (report.Outcome, report.Subject.Kind == SubjectKind.Resource)))
            {
                ConclusionReport[] values = group
                    .OrderBy(report => report.ConclusionId, StringComparer.Ordinal)
                    .ToArray();
                if (values.Length > MaximumSubjectsPerCard)
                {
                    string qualifier = group.Key switch
                    {
                        (ComponentOutcome.Supports, false) => "Many nearby rares: ",
                        (ComponentOutcome.PreferenceSensitive, false) => "Many rares: ",
                        (ComponentOutcome.DoesNotSupport, false) => "Many distant rares: ",
                        (ComponentOutcome.DoesNotSupport, true) =>
                            "Many rare resources absent: ",
                        _ => String.Empty
                    };
                    if (qualifier.Length != 0)
                    {
                        IEnumerable<string> examples = group.Key.IsAbsent
                            ? values.Select(value => ResourceLabel(value.ConclusionId))
                            : values.Select(value =>
                                displays.TryGetValue(
                                    value.Subject.Identifier,
                                    out RuntimeSystemDisplay? display)
                                    ? ResourceLabel(value.ConclusionId) + " in " +
                                        display.DisplayName
                                    : ResourceLabel(value.ConclusionId));
                        AddMegaCard(cards, values, qualifier + JoinNames(examples));
                    }
                    continue;
                }

                foreach (ConclusionReport report in values)
                {
                    string resource = ResourceLabel(report.ConclusionId);
                    if (report.Subject.Kind == SubjectKind.Resource)
                    {
                        AddMegaCard(cards, new[] { report }, "No " + resource);
                        continue;
                    }
                    if (!displays.TryGetValue(
                        report.Subject.Identifier,
                        out RuntimeSystemDisplay? display))
                    {
                        continue;
                    }
                    string description = report.Outcome switch
                    {
                        ComponentOutcome.Supports => "nearby " + resource,
                        ComponentOutcome.PreferenceSensitive => resource,
                        ComponentOutcome.DoesNotSupport => "distant " + resource,
                        _ => String.Empty
                    };
                    if (description.Length != 0)
                    {
                        AddMegaRole(
                            roles,
                            new RuntimeSystemCandidate(
                                report.Subject.Identifier,
                                display.DisplayName,
                                0m),
                            report.Outcome,
                            description,
                            report);
                    }
                }
            }
        }

        private static ConclusionReport? SingleReport(
            IEnumerable<ConclusionReport> reports,
            string conclusionId) => reports.SingleOrDefault(report =>
                report.ConclusionId == conclusionId);

        private static void AddMegaRole(
            IDictionary<string, MegaSystemCard> roles,
            RuntimeSystemCandidate candidate,
            ComponentOutcome outcome,
            string description,
            params ConclusionReport?[] sources)
        {
            string key = ((int)outcome).ToString(CultureInfo.InvariantCulture) + "\t" +
                candidate.Identifier;
            if (!roles.TryGetValue(key, out MegaSystemCard? role))
            {
                role = new MegaSystemCard(candidate.DisplayName, outcome);
                roles.Add(key, role);
            }
            if (!role.Descriptions.Contains(description, StringComparer.Ordinal))
                role.Descriptions.Add(description);
            foreach (ConclusionReport? source in sources.Where(value => value != null))
            {
                if (!role.Sources.Contains(source!))
                    role.Sources.Add(source!);
            }
        }

        private static string SingleMegaRoleLine(string system, string description)
        {
            if (description.StartsWith("nearby ", StringComparison.Ordinal))
                return "Nearby " + description.Substring("nearby ".Length) + " in " + system;
            if (description.StartsWith("distant ", StringComparison.Ordinal))
                return "Distant " + description.Substring("distant ".Length) + " in " + system;
            if (description == "large sphere")
                return "Large sphere at " + system;
            if (description.EndsWith("contained orbit", StringComparison.Ordinal) ||
                description.EndsWith("contained orbits", StringComparison.Ordinal))
                return Char.ToUpperInvariant(description[0]) + description.Substring(1) +
                    " at " + system;
            return system + " " + description;
        }

        private static void AddMegaCard(
            ICollection<PresentedConclusionCard> cards,
            IEnumerable<ConclusionReport> sources,
            string line)
        {
            ConclusionReport[] values = sources.ToArray();
            if (values.Length == 0)
                return;
            cards.Add(new PresentedConclusionCard(
                ConclusionContext.Megafactory,
                values[0].Stage,
                values[0].Outcome,
                "MEGAFACTORY-CANDIDATES",
                line,
                Array.Empty<string>(),
                Bound(line, MaximumLineCharacters),
                values.Select(value => value.ConclusionId)));
        }

        private sealed class MegaSystemCard
        {
            public MegaSystemCard(string displayName, ComponentOutcome outcome)
            {
                DisplayName = displayName;
                Outcome = outcome;
            }

            public string DisplayName { get; }
            public ComponentOutcome Outcome { get; }
            public List<string> Descriptions { get; } = new List<string>();
            public List<ConclusionReport> Sources { get; } = new List<ConclusionReport>();
        }

        private static void AddFreshCard(
            ICollection<PresentedConclusionCard> destination,
            IReadOnlyList<ConclusionReport> sources,
            string? line)
        {
            if (line == null || sources.Count == 0)
                return;
            ComponentOutcome outcome = sources[0].Outcome;
            destination.Add(new PresentedConclusionCard(
                ConclusionContext.FreshStart,
                sources[0].Stage,
                outcome,
                Family(sources[0].ConclusionId),
                line,
                Array.Empty<string>(),
                Bound(line, MaximumLineCharacters),
                sources.Select(source => source.ConclusionId)));
        }

        private static ConclusionReport[] Reports(
            IEnumerable<ConclusionReport> reports,
            string conclusionId)
        {
            return reports.Where(report => report.ConclusionId == conclusionId).ToArray();
        }

        private static string? GasLine(
            IReadOnlyList<ConclusionReport> reports,
            IReadOnlyList<NormalizedBirthPlanetEvidence>? birthPlanets)
        {
            if (birthPlanets == null)
                return null;
            NormalizedBirthPlanetEvidence[] giants = birthPlanets
                .Where(planet => planet.IsGasGiant)
                .ToArray();
            if (giants.Length == 0)
                return null;

            if (reports.Count == 0)
                return null;
            string[] products = reports
                .Where(report => giants.Any(giant =>
                    giant.GasProductIds.Contains(
                        report.ConclusionId.Substring(
                            "FS-GAS-ROUTE.product:".Length),
                        StringComparer.Ordinal)) ==
                    (report.Outcome == ComponentOutcome.Supports))
                .Select(report => ResourceLabel(report.ConclusionId))
                .ToArray();
            if (products.Length == 0)
                return null;
            bool present = reports[0].Outcome == ComponentOutcome.Supports;
            string subject = giants.Length == 1
                ? "Starter gas giant"
                : "Starter gas giants";
            string verb = present
                ? (giants.Length == 1 ? " has " : " have ")
                : (giants.Length == 1 ? " lacks " : " lack ");
            return subject + verb + String.Join(" / ", products);
        }

        private static string? PowerLine(
            IReadOnlyList<ConclusionReport> reports,
            IReadOnlyList<NormalizedBirthPlanetEvidence>? birthPlanets,
            string conclusionId,
            string noun,
            string strong,
            string middle,
            string weak,
            Func<NormalizedBirthPlanetEvidence, decimal?> selectValue)
        {
            ConclusionReport? report = reports.SingleOrDefault(value =>
                value.ConclusionId == conclusionId);
            if (report == null || birthPlanets == null)
                return null;
            AcceptedRange range = noun == "solar"
                ? ConclusionDefinition.Solar
                : ConclusionDefinition.Wind;
            NormalizedBirthPlanetEvidence[] matching = birthPlanets
                .Where(planet => !planet.IsGasGiant && selectValue(planet).HasValue &&
                    ConclusionDefinition.Evaluate(selectValue(planet)!.Value, range) ==
                        report.Outcome)
                .OrderBy(planet => planet.PlanetId)
                .ToArray();
            if (matching.Length == 0)
                return null;
            string adjective = report.Outcome switch
            {
                ComponentOutcome.Supports => strong,
                ComponentOutcome.PreferenceSensitive => middle,
                ComponentOutcome.DoesNotSupport => weak,
                _ => throw new ArgumentOutOfRangeException(nameof(report.Outcome))
            };
            if (matching.Length > MaximumSubjectsPerCard)
                return "Many planets have " + adjective + " " + noun;
            return JoinNames(matching.Select(planet => planet.DisplayName)) +
                (matching.Length == 1 ? " has " : " have ") + adjective + " " + noun;
        }

        private static string? TidalLine(
            IReadOnlyList<ConclusionReport> reports,
            IReadOnlyList<NormalizedBirthPlanetEvidence>? birthPlanets)
        {
            ConclusionReport? report = reports.SingleOrDefault(value =>
                value.ConclusionId == "FS-POWER.birth-tidal");
            if (report == null || birthPlanets == null)
                return null;
            if (report.Outcome == ComponentOutcome.DoesNotSupport)
                return "No permanent solar sources";
            NormalizedBirthPlanetEvidence[] tidal = birthPlanets
                .Where(planet => !planet.IsGasGiant && planet.IsTidalLocked == true)
                .OrderBy(planet => planet.PlanetId)
                .ToArray();
            return tidal.Length == 0
                ? null
                : tidal.Length > MaximumSubjectsPerCard
                    ? "Many permanent solar sources"
                : (tidal.Length == 1 ? "Permanent solar source on " :
                    "Permanent solar sources on ") +
                    JoinNames(tidal.Select(planet => planet.DisplayName));
        }

        private static string? TopologyLine(IReadOnlyList<ConclusionReport> reports)
        {
            ConclusionReport? report = reports.SingleOrDefault(value =>
                value.ConclusionId == SharedSatelliteEvaluator.ConclusionId);
            if (report?.DecisiveFact == null ||
                !Int32.TryParse(
                    report.DecisiveFact.Value,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out int sharedBodies))
            {
                return null;
            }
            int neighbors = Math.Max(0, sharedBodies - 1);
            return neighbors == 0
                ? "No gas giant neighbors"
                : neighbors.ToString(CultureInfo.InvariantCulture) +
                    (neighbors == 1 ? " gas giant neighbor" : " gas giant neighbors");
        }

        private static string? ResourceLine(ConclusionReport report)
        {
            string resource = ResourceLabel(report.ConclusionId);
            if (report.ConclusionId.StartsWith("FS-RESOURCES.amount:", StringComparison.Ordinal))
            {
                return resource + " " + OutcomeWord(
                    report.Outcome,
                    "plentiful",
                    "normal",
                    "scarce");
            }
            if (report.ConclusionId.StartsWith("FS-RESOURCES.groups:", StringComparison.Ordinal))
            {
                return resource + " has " + OutcomeWord(
                    report.Outcome,
                    "many vein groups",
                    "normal vein groups",
                    "few vein groups");
            }
            if (report.ConclusionId == "FS-RESOURCES.fire-ice")
            {
                return report.Outcome == ComponentOutcome.Supports
                    ? "Found Fire Ice veins"
                    : "No Fire Ice veins";
            }
            return null;
        }

        private static string OutcomeWord(
            ComponentOutcome outcome,
            string strong,
            string middle,
            string weak) => outcome switch
            {
                ComponentOutcome.Supports => strong,
                ComponentOutcome.PreferenceSensitive => middle,
                ComponentOutcome.DoesNotSupport => weak,
                _ => throw new ArgumentOutOfRangeException(nameof(outcome))
            };

        private static string JoinNames(IEnumerable<string> names)
        {
            string[] values = names.Take(MaximumSubjectsPerCard).ToArray();
            return values.Length switch
            {
                0 => String.Empty,
                1 => values[0],
                2 => values[0] + " and " + values[1],
                _ => values[0] + ", " + values[1] + ", and " + values[2]
            };
        }

        private static void AddSection(
            ICollection<PreviewPanelLine> lines,
            string title,
            IReadOnlyList<PresentedContextGroup> groups)
        {
            lines.Add(new PreviewPanelLine(PreviewPanelLineKind.Section, title));
            AddGroups(lines, groups);
        }

        private static void AddGroups(
            ICollection<PreviewPanelLine> lines,
            IReadOnlyList<PresentedContextGroup> groups)
        {
            foreach (PresentedContextGroup group in groups)
            {
                lines.Add(new PreviewPanelLine(PreviewPanelLineKind.Context, group.Title));
                foreach (PresentedConclusionCard card in group.Cards)
                    lines.Add(new PreviewPanelLine(PreviewPanelLineKind.Conclusion, card.Line));
            }
        }

        private static string IdentityLine(PreviewGenerationIdentity identity)
        {
            GenerationIdentity galaxy = identity.GalaxyIdentity;
            string value = "Seed " + galaxy.GalaxySeed.ToString("D8", CultureInfo.InvariantCulture) +
                " | " + galaxy.RequestedStarCount.ToString(CultureInfo.InvariantCulture) +
                " stars | resources x" + identity.ResourceMultiplier.ToString(
                    "G29",
                    CultureInfo.InvariantCulture) +
                " | " + (identity.CombatMode == CombatMode.Peace ? "Peace" : "Combat");
            return Bound(value, MaximumLineCharacters);
        }

        private static string? DarkFogStatusLine(RuntimeDarkFogOccupation? occupation)
        {
            if (occupation == null)
                return null;

            int total = occupation.ClusterInitialHiveCount;
            int starter = occupation.BirthSystemInitialHiveCount;
            string totalText = total.ToString(CultureInfo.InvariantCulture) +
                (total == 1 ? " initial hive" : " initial hives");
            string starterText = starter == 0
                ? "none in starter system"
                : starter.ToString(CultureInfo.InvariantCulture) + " in starter system";
            return Bound("Dark Fog: " + totalText + "; " + starterText,
                MaximumLineCharacters);
        }

        private static string ContextTitle(ConclusionContext context) => context switch
        {
            ConclusionContext.FreshStart => "Fresh start",
            ConclusionContext.Megafactory => "Megafactory",
            ConclusionContext.CompactExpansion => "Compact expansion",
            ConclusionContext.SphereShowcase => "Sphere / energy",
            ConclusionContext.DecisionRelevantTraits => "Decision-relevant traits",
            _ => throw new ArgumentOutOfRangeException(nameof(context))
        };

        private static string Family(string conclusionId)
        {
            string[] accepted =
            {
                "FS-TOPOLOGY",
                "FS-POWER",
                "FS-GAS-ROUTE",
                "FS-RESOURCES",
                "MF-ENERGY-SYSTEM",
                "MF-SPHERE-GEOMETRY",
                "MF-SYSTEM-ROLE",
                "MF-RESOURCE-SCOPE",
                "CX-GROUPING",
                "RR-ACCESS",
                "TRAIT-SUMMARY"
            };
            foreach (string family in accepted)
            {
                if (conclusionId == family ||
                    conclusionId.StartsWith(family + ".", StringComparison.Ordinal))
                {
                    return family;
                }
            }
            throw new InvalidOperationException(
                "No presentation mapping exists for conclusion " + conclusionId + ".");
        }

        private static string FamilyTitle(string family, ConclusionReport report)
        {
            return family switch
            {
                "FS-TOPOLOGY" => "Birth-system topology",
                "FS-POWER" => "Birth-system renewable power",
                "FS-GAS-ROUTE" => "Birth-system gas products",
                "FS-RESOURCES" => "Starter resources",
                "MF-ENERGY-SYSTEM" => "Stellar-energy candidate",
                "MF-SPHERE-GEOMETRY" => "Sphere geometry",
                "MF-SYSTEM-ROLE" => "Supported system roles",
                "MF-RESOURCE-SCOPE" => "Cluster resource scale",
                "CX-GROUPING" => "Supported-role grouping",
                "RR-ACCESS" => "Rare-resource access",
                "TRAIT-SUMMARY" => "Decision-relevant traits",
                _ => throw new ArgumentOutOfRangeException(nameof(family))
            };
        }

        private static string OutcomeLabel(ComponentOutcome outcome) => outcome switch
        {
            ComponentOutcome.Supports => "Strength",
            ComponentOutcome.DoesNotSupport => "Limitation",
            ComponentOutcome.PreferenceSensitive => "Preference-sensitive",
            ComponentOutcome.Tradeoff => "Tradeoff",
            ComponentOutcome.Caution => "Caution",
            ComponentOutcome.Unknown => "Unknown",
            ComponentOutcome.NotApplicable => "Not applicable",
            _ => throw new ArgumentOutOfRangeException(nameof(outcome))
        };

        private static string? SubjectLabel(
            ConclusionReport report,
            IReadOnlyDictionary<string, RuntimeSystemDisplay> displays)
        {
            string? semantic = SemanticSubjectLabel(report, displays);
            if (semantic != null)
                return semantic;

            string identifier = report.Subject.Identifier;
            return report.Subject.Kind switch
            {
                SubjectKind.BirthSystem => SystemLabel(identifier, displays) ??
                    "Birth system",
                SubjectKind.StarSystem => SystemLabel(identifier, displays),
                SubjectKind.Cluster => "Cluster",
                SubjectKind.Resource => ResourceLabel(report.ConclusionId),
                SubjectKind.SystemPair => PairLabel(identifier, displays),
                SubjectKind.Trait => Pretty(identifier.Split('@')[0]),
                _ => throw new ArgumentOutOfRangeException(nameof(report.Subject.Kind))
            };
        }

        private static string? SemanticSubjectLabel(
            ConclusionReport report,
            IReadOnlyDictionary<string, RuntimeSystemDisplay> displays)
        {
            string id = report.ConclusionId;
            string? system = BasicSubjectLabel(report.Subject, displays);
            if (id == "FS-POWER.birth-tidal")
                return "Tidal lock @ " + (system ?? "Birth system");
            if (id == "FS-POWER.solar")
                return "Solar @ " + (system ?? "Birth system");
            if (id == "FS-POWER.wind")
                return "Wind @ " + (system ?? "Birth system");
            if (id.StartsWith("FS-GAS-ROUTE.product:", StringComparison.Ordinal))
                return ResourceLabel(id) + " presence";
            if (id.StartsWith("FS-GAS-ROUTE.rate:", StringComparison.Ordinal))
                return ResourceLabel(id) + " collection rate";
            if (id == "FS-RESOURCES.common-total")
                return "Combined starter deposits";
            if (id.StartsWith("FS-RESOURCES.amount:", StringComparison.Ordinal))
                return ResourceLabel(id) + " amount";
            if (id.StartsWith("FS-RESOURCES.groups:", StringComparison.Ordinal))
                return ResourceLabel(id) + " distribution";
            if (id == "FS-RESOURCES.fire-ice")
                return "Fire Ice presence";
            if (id == "MF-ENERGY-SYSTEM.output")
                return system == null ? null : "Output @ " + system;
            if (id == "MF-ENERGY-SYSTEM.separation")
                return system == null ? null : "Leader separation @ " + system;
            if (id == "MF-SPHERE-GEOMETRY.radius")
                return system == null ? null : "Shell radius @ " + system;
            if (id == "MF-SPHERE-GEOMETRY.containment")
                return system == null ? null : "Contained orbits @ " + system;
            if (id.StartsWith("MF-SYSTEM-ROLE.role:", StringComparison.Ordinal))
                return system == null ? null :
                    Pretty(id.Substring("MF-SYSTEM-ROLE.role:".Length)) + " @ " + system;
            if (id == "MF-RESOURCE-SCOPE.strength")
                return "Complete cluster";
            if (id.StartsWith("RR-ACCESS.distance:", StringComparison.Ordinal))
            {
                string? distance = DistanceLabel(report.DecisiveFact);
                if (distance == null)
                    return null;
                string location = report.Subject.Kind == SubjectKind.StarSystem ||
                    report.Subject.Kind == SubjectKind.BirthSystem
                    ? SystemLabel(report.Subject.Identifier, displays) ?? "Birth system"
                    : String.Empty;
                return ResourceLabel(id) + " - " + distance + " from birth" +
                    (location.Length == 0 ? String.Empty : " @ " + location);
            }
            if (id == "CX-GROUPING.distance")
            {
                string? distance = DistanceLabel(report.DecisiveFact);
                string? pair = PairLabel(report.Subject.Identifier, displays);
                return distance == null || pair == null
                    ? null
                    : distance + " between " + pair;
            }
            if (id.StartsWith("RR-ACCESS.amount:", StringComparison.Ordinal))
                return ResourceLabel(id) + " amount";
            return null;
        }

        private static string? BasicSubjectLabel(
            ConclusionSubject subject,
            IReadOnlyDictionary<string, RuntimeSystemDisplay> displays) =>
            subject.Kind switch
            {
                SubjectKind.BirthSystem => SystemLabel(subject.Identifier, displays) ??
                    "Birth system",
                SubjectKind.StarSystem => SystemLabel(subject.Identifier, displays),
                SubjectKind.Cluster => "Cluster",
                _ => null
            };

        private static string ResourceLabel(string conclusionId)
        {
            int separator = conclusionId.LastIndexOf(':');
            return separator >= 0
                ? Pretty(conclusionId.Substring(separator + 1))
                : "Resource";
        }

        private static string? PairLabel(
            string identifier,
            IReadOnlyDictionary<string, RuntimeSystemDisplay> displays)
        {
            int separator = identifier.IndexOf("<->", StringComparison.Ordinal);
            if (separator < 0)
                return null;
            string first = identifier.Substring(0, separator);
            string remaining = identifier.Substring(separator + 3);
            int roles = remaining.LastIndexOf(':');
            string second = roles < 0 ? remaining : remaining.Substring(0, roles);
            string? firstLabel = SystemLabel(first, displays);
            string? secondLabel = SystemLabel(second, displays);
            return firstLabel == null || secondLabel == null
                ? null
                : firstLabel + " / " + secondLabel;
        }

        private static string? SystemLabel(
            string identifier,
            IReadOnlyDictionary<string, RuntimeSystemDisplay> displays)
        {
            if (!displays.TryGetValue(identifier, out RuntimeSystemDisplay? display))
                return null;
            return display.DisplayName + " (" + display.StarType + ")";
        }

        private static string? DistanceLabel(DecisiveFact? fact)
        {
            if (fact == null ||
                !String.Equals(fact.Unit, "light-years", StringComparison.Ordinal) ||
                !Decimal.TryParse(
                    fact.Value,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal value))
            {
                return null;
            }
            string format = Math.Abs(value) >= 100m ? "0" :
                Math.Abs(value) >= 10m ? "0.0" :
                Math.Abs(value) >= 1m ? "0.00" : "0.000";
            return value.ToString(format, CultureInfo.InvariantCulture) + " ly";
        }

        private static string SubjectSummary(IReadOnlyList<string> subjects)
        {
            if (subjects.Count == 0)
                return "Current identity";
            string shown = String.Join(", ", subjects.Take(MaximumSubjectsPerCard));
            if (subjects.Count > MaximumSubjectsPerCard)
                shown += " +" + (subjects.Count - MaximumSubjectsPerCard).ToString(
                    CultureInfo.InvariantCulture);
            return shown;
        }

        private static string Pretty(string value)
        {
            string[] words = value.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
            return String.Join(" ", words.Select(word =>
                word.Length == 0
                    ? word
                    : Char.ToUpperInvariant(word[0]) + word.Substring(1)));
        }

        private static string Bound(string value, int maximum)
        {
            if (value.Length <= maximum)
                return value;
            return value.Substring(0, maximum - 3).TrimEnd() + "...";
        }
    }
}
