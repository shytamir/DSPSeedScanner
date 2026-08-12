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
            Family = family;
            Title = title;
            Subjects = Array.AsReadOnly(subjects.ToArray());
            Line = line;
            SourceConclusionIds = Array.AsReadOnly(sourceConclusionIds.ToArray());
        }

        public ConclusionContext Context { get; }
        public EvidenceStage Stage { get; }
        public ComponentOutcome Outcome { get; }
        public string Family { get; }
        public string Title { get; }
        public IReadOnlyList<string> Subjects { get; }
        public string Line { get; }
        public IReadOnlyList<string> SourceConclusionIds { get; }
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
            string detailSectionTitle,
            bool isCached,
            IEnumerable<PresentedContextGroup> immediateGroups,
            IEnumerable<PresentedContextGroup> detailGroups)
        {
            SessionId = sessionId;
            IdentityLine = identityLine;
            DetailSectionTitle = detailSectionTitle;
            IsCached = isCached;
            ImmediateGroups = Array.AsReadOnly(immediateGroups.ToArray());
            DetailGroups = Array.AsReadOnly(detailGroups.ToArray());
        }

        public long SessionId { get; }
        public string IdentityLine { get; }
        public string DetailSectionTitle { get; }
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
        public const int MaximumLineCharacters = 112;
        public const int MaximumDocumentLines = 72;
        public const int MaximumSubjectsPerCard = 3;

        private static readonly ConclusionContext[] ContextOrder =
        {
            ConclusionContext.FreshStart,
            ConclusionContext.Megafactory,
            ConclusionContext.DarkFogFarming,
            ConclusionContext.CompactExpansion,
            ConclusionContext.SphereShowcase,
            ConclusionContext.DecisionRelevantTraits
        };

        public static PreviewConclusionPresentation Project(PreviewResolutionAttempt attempt)
        {
            if (attempt == null)
                throw new ArgumentNullException(nameof(attempt));

            PreviewGenerationIdentity identity = attempt.Session.Identity;
            return new PreviewConclusionPresentation(
                attempt.Session.SessionId,
                IdentityLine(identity),
                DetailTitle(attempt.State),
                attempt.State == PreviewResolutionState.Cached,
                Group(attempt.PreviewReports.Where(report =>
                    report.Stage == EvidenceStage.GalaxyPreview)),
                Group(attempt.CompleteReports.Where(report =>
                    report.Stage == EvidenceStage.CompleteClusterRaw)));
        }

        public static PresentedConclusionCard MapCard(ConclusionReport report)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));
            return BuildCard(new[] { report });
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

            AddSection(lines, "Immediate preview", conclusions.ImmediateGroups);
            lines.Add(new PreviewPanelLine(
                PreviewPanelLineKind.Section,
                conclusions.DetailSectionTitle));
            AddGroups(lines, conclusions.DetailGroups);
            return new PreviewPanelDocument(lines);
        }

        private static IReadOnlyList<PresentedContextGroup> Group(
            IEnumerable<ConclusionReport> source)
        {
            ConclusionReport[] reports = source.ToArray();
            var groups = new List<PresentedContextGroup>();
            foreach (ConclusionContext context in ContextOrder)
            {
                ConclusionReport[] contextReports = reports
                    .Where(report => report.Context == context)
                    .ToArray();
                if (contextReports.Length == 0)
                    continue;

                var cards = new List<PresentedConclusionCard>();
                var ordinary = new Dictionary<string, List<ConclusionReport>>(StringComparer.Ordinal);
                var order = new List<string>();
                foreach (ConclusionReport report in contextReports)
                {
                    if (report.Outcome == ComponentOutcome.Tradeoff ||
                        report.Outcome == ComponentOutcome.Caution)
                    {
                        cards.Add(BuildCard(new[] { report }));
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
                    cards.Add(BuildCard(ordinary[key]));

                groups.Add(new PresentedContextGroup(context, ContextTitle(context), cards));
            }
            return Array.AsReadOnly(groups.ToArray());
        }

        private static PresentedConclusionCard BuildCard(
            IReadOnlyList<ConclusionReport> reports)
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
                .Select(SubjectLabel)
                .Distinct(StringComparer.Ordinal)
                .ToArray();
            string title = FamilyTitle(family, first);
            string line = Bound(
                OutcomeLabel(first.Outcome) + ": " + title + " - " +
                SubjectSummary(subjects),
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

        private static string DetailTitle(PreviewResolutionState state) => state switch
        {
            PreviewResolutionState.Scanning => "Detailed conclusions - scanning",
            PreviewResolutionState.Cached => "Detailed conclusions - cached",
            PreviewResolutionState.Complete => "Detailed conclusions - complete",
            _ => "Detailed conclusions - unavailable"
        };

        private static string ContextTitle(ConclusionContext context) => context switch
        {
            ConclusionContext.FreshStart => "Fresh start",
            ConclusionContext.Megafactory => "Megafactory",
            ConclusionContext.DarkFogFarming => "Dark Fog farming",
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
                "DF-OCCUPATION",
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
            if (report.Outcome == ComponentOutcome.Tradeoff && family == "DF-OCCUPATION")
                return "Farming opportunity and birth exposure";
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
                "DF-OCCUPATION" => "Generated Dark Fog occupation",
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

        private static string SubjectLabel(ConclusionReport report)
        {
            string? semantic = SemanticSubjectLabel(report);
            if (semantic != null)
                return semantic;

            string identifier = report.Subject.Identifier;
            return report.Subject.Kind switch
            {
                SubjectKind.BirthSystem => "Birth system",
                SubjectKind.StarSystem => "System " + identifier,
                SubjectKind.Cluster => "Cluster",
                SubjectKind.Resource => ResourceLabel(report.ConclusionId),
                SubjectKind.SystemPair => "Systems " + PairLabel(identifier),
                SubjectKind.Trait => Pretty(identifier.Split('@')[0]),
                _ => throw new ArgumentOutOfRangeException(nameof(report.Subject.Kind))
            };
        }

        private static string? SemanticSubjectLabel(ConclusionReport report)
        {
            string id = report.ConclusionId;
            if (id == "FS-POWER.birth-tidal")
                return "Tidal lock @ Birth system";
            if (id == "FS-POWER.solar")
                return "Solar @ Birth system";
            if (id == "FS-POWER.wind")
                return "Wind @ Birth system";
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
                return "Output @ " + BasicSubjectLabel(report.Subject);
            if (id == "MF-ENERGY-SYSTEM.separation")
                return "Leader separation @ " + BasicSubjectLabel(report.Subject);
            if (id == "MF-SPHERE-GEOMETRY.radius")
                return "Shell radius @ " + BasicSubjectLabel(report.Subject);
            if (id == "MF-SPHERE-GEOMETRY.containment")
                return "Contained orbits @ " + BasicSubjectLabel(report.Subject);
            if (id.StartsWith("MF-SYSTEM-ROLE.role:", StringComparison.Ordinal))
                return Pretty(id.Substring("MF-SYSTEM-ROLE.role:".Length)) + " @ " +
                    BasicSubjectLabel(report.Subject);
            if (id == "MF-RESOURCE-SCOPE.strength")
                return "Complete cluster";
            if (id == "DF-OCCUPATION.opportunity")
                return "Cluster opportunity";
            if (id == "DF-OCCUPATION.birth-exposure")
                return "Birth-system exposure";
            if (id.StartsWith("RR-ACCESS.distance:", StringComparison.Ordinal))
                return ResourceLabel(id) + " distance";
            if (id.StartsWith("RR-ACCESS.amount:", StringComparison.Ordinal))
                return ResourceLabel(id) + " amount";
            return null;
        }

        private static string BasicSubjectLabel(ConclusionSubject subject) =>
            subject.Kind switch
            {
                SubjectKind.BirthSystem => "Birth system",
                SubjectKind.StarSystem => "System " + subject.Identifier,
                SubjectKind.Cluster => "Cluster",
                _ => subject.Identifier
            };

        private static string ResourceLabel(string conclusionId)
        {
            int separator = conclusionId.LastIndexOf(':');
            return separator >= 0
                ? Pretty(conclusionId.Substring(separator + 1))
                : "Resource";
        }

        private static string PairLabel(string identifier)
        {
            int roles = identifier.IndexOf(':');
            string pair = roles < 0 ? identifier : identifier.Substring(0, roles);
            return pair.Replace("<->", " / ");
        }

        private static string SubjectSummary(IReadOnlyList<string> subjects)
        {
            if (subjects.Count == 0)
                return "Current identity";
            string shown = String.Join(", ", subjects.Take(MaximumSubjectsPerCard));
            if (subjects.Count > MaximumSubjectsPerCard)
                shown += " +" + (subjects.Count - MaximumSubjectsPerCard).ToString(
                    CultureInfo.InvariantCulture);
            return Bound(shown, 40);
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
