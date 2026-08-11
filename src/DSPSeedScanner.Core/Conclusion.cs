using System;

namespace DSPSeedScanner.Core
{
    public enum SubjectKind
    {
        BirthSystem,
        StarSystem,
        Cluster,
        Resource,
        SystemPair,
        Trait
    }

    public enum ConclusionContext
    {
        FreshStart,
        Megafactory,
        DarkFogFarming,
        CompactExpansion,
        SphereShowcase,
        DecisionRelevantTraits
    }

    public enum ComponentOutcome
    {
        Supports,
        DoesNotSupport,
        PreferenceSensitive,
        Tradeoff,
        Caution,
        Unknown,
        NotApplicable
    }

    public sealed record ConclusionSubject
    {
        public ConclusionSubject(SubjectKind kind, string identifier)
        {
            if (String.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("Identifier is required.", nameof(identifier));

            Kind = kind;
            Identifier = identifier;
        }

        public SubjectKind Kind { get; }

        public string Identifier { get; }
    }

    public sealed record DecisiveFact
    {
        public DecisiveFact(string factId, string value, string unit)
        {
            FactId = Required(factId, nameof(factId));
            Value = Required(value, nameof(value));
            Unit = Required(unit, nameof(unit));
        }

        public string FactId { get; }

        public string Value { get; }

        public string Unit { get; }

        private static string Required(string value, string parameterName)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value is required.", parameterName);

            return value;
        }
    }

    public sealed record DiagnosticCause
    {
        public DiagnosticCause(string code, string message)
        {
            if (String.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code is required.", nameof(code));
            if (String.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message is required.", nameof(message));

            Code = code;
            Message = message;
        }

        public string Code { get; }

        public string Message { get; }
    }

    public sealed record ConclusionReport
    {
        public ConclusionReport(
            GenerationIdentity identity,
            EvaluationSettings settings,
            EvidenceCoverage coverage,
            string conclusionId,
            ConclusionContext context,
            string contractVersion,
            string definitionVersion,
            ConclusionSubject subject,
            ComponentOutcome outcome,
            DecisiveFact? decisiveFact,
            DiagnosticCause? diagnosticCause,
            string? sourceConclusionId = null)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Coverage = coverage ?? throw new ArgumentNullException(nameof(coverage));
            ConclusionId = Required(conclusionId, nameof(conclusionId));
            Context = context;
            ContractVersion = Required(contractVersion, nameof(contractVersion));
            DefinitionVersion = Required(definitionVersion, nameof(definitionVersion));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Outcome = outcome;
            if ((outcome == ComponentOutcome.Unknown ||
                 outcome == ComponentOutcome.NotApplicable) && diagnosticCause == null)
            {
                throw new ArgumentException(
                    "Unknown and not-applicable outcomes require a diagnostic cause.",
                    nameof(diagnosticCause));
            }
            if (outcome != ComponentOutcome.Unknown &&
                outcome != ComponentOutcome.NotApplicable && diagnosticCause != null)
            {
                throw new ArgumentException(
                    "Resolved outcomes cannot carry a diagnostic cause.",
                    nameof(diagnosticCause));
            }
            DecisiveFact = decisiveFact;
            DiagnosticCause = diagnosticCause;
            SourceConclusionId = Optional(sourceConclusionId, nameof(sourceConclusionId));
        }

        public GenerationIdentity Identity { get; }

        public EvidenceStage Stage => Coverage.Stage;

        public EvaluationSettings Settings { get; }

        public EvidenceCoverage Coverage { get; }

        public string ConclusionId { get; }

        public ConclusionContext Context { get; }

        public string ContractVersion { get; }

        public string DefinitionVersion { get; }

        public ConclusionSubject Subject { get; }

        public ComponentOutcome Outcome { get; }

        public DecisiveFact? DecisiveFact { get; }

        public DiagnosticCause? DiagnosticCause { get; }

        public string? SourceConclusionId { get; }

        private static string Required(string value, string parameterName)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value is required.", parameterName);

            return value;
        }

        private static string? Optional(string? value, string parameterName)
        {
            if (value != null && String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be blank.", parameterName);

            return value;
        }
    }
}
