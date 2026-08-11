using System;

namespace DSPSeedScanner.Core
{
    public enum SubjectKind
    {
        BirthSystem
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
            EvidenceCoverage coverage,
            string conclusionId,
            string contractVersion,
            string definitionVersion,
            ConclusionSubject subject,
            ComponentOutcome outcome,
            DecisiveFact? decisiveFact,
            DiagnosticCause? diagnosticCause)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Coverage = coverage ?? throw new ArgumentNullException(nameof(coverage));
            ConclusionId = Required(conclusionId, nameof(conclusionId));
            ContractVersion = Required(contractVersion, nameof(contractVersion));
            DefinitionVersion = Required(definitionVersion, nameof(definitionVersion));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Outcome = outcome;
            DecisiveFact = decisiveFact;
            DiagnosticCause = diagnosticCause;
        }

        public GenerationIdentity Identity { get; }

        public EvidenceStage Stage => Coverage.Stage;

        public EvidenceCoverage Coverage { get; }

        public string ConclusionId { get; }

        public string ContractVersion { get; }

        public string DefinitionVersion { get; }

        public ConclusionSubject Subject { get; }

        public ComponentOutcome Outcome { get; }

        public DecisiveFact? DecisiveFact { get; }

        public DiagnosticCause? DiagnosticCause { get; }

        private static string Required(string value, string parameterName)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value is required.", parameterName);

            return value;
        }
    }
}
