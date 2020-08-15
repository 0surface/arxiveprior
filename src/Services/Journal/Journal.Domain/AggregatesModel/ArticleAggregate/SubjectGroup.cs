using Journal.Domain.SeedWork;

namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class SubjectGroup
        : Enumeration
    {
        public static SubjectGroup Astrophysics = new SubjectGroup(1, "astro-ph", Discipline.Physics);
        public static SubjectGroup CondensedMatter = new SubjectGroup(2, "cond-mat", Discipline.Physics);
        public static SubjectGroup GeneralRelativityQuantumCosmology = new SubjectGroup(3, "gr-qc", Discipline.Physics);
        public static SubjectGroup HighEnergyPhysicsExperiment = new SubjectGroup(4, "hep-ex", Discipline.Physics);
        public static SubjectGroup HighEnergyPhysicsLattice = new SubjectGroup(5, "hep-lat", Discipline.Physics);
        public static SubjectGroup HighEnergyPhysicsPhenomenology = new SubjectGroup(6, "hep-ph", Discipline.Physics);
        public static SubjectGroup HighEnergyPhysicsTheory = new SubjectGroup(7, "hep-th", Discipline.Physics);
        public static SubjectGroup MathematicalPhysics = new SubjectGroup(8, "math-ph", Discipline.Physics);
        public static SubjectGroup NonlinearSciences = new SubjectGroup(9, "nlin", Discipline.Physics);
        public static SubjectGroup NuclearExperiment = new SubjectGroup(10, "nucl-ex", Discipline.Physics);
        public static SubjectGroup NuclearTheory = new SubjectGroup(11, "nucl-th", Discipline.Physics);
        public static SubjectGroup Physics = new SubjectGroup(12, "physics", Discipline.Physics);
        public static SubjectGroup QuantumPhysics = new SubjectGroup(13, "quant-ph", Discipline.Physics);
        public static SubjectGroup Mathematics = new SubjectGroup(14, "math", Discipline.Mathematics);
        public static SubjectGroup ComputerScience = new SubjectGroup(15, "cs", Discipline.ComputerScience);
        public static SubjectGroup QuantitativeBiology = new SubjectGroup(16, "q-bio", Discipline.QuantitativeBiology);
        public static SubjectGroup QuantitativeFinance = new SubjectGroup(17, "q-fin", Discipline.QuantitativeFinance);
        public static SubjectGroup Statistics = new SubjectGroup(18, "stat", Discipline.Statistics);
        public static SubjectGroup ElectricalEngineeringSystemsScience = new SubjectGroup(19, "eess", Discipline.EESS);
        public static SubjectGroup Economics = new SubjectGroup(20, "econ", Discipline.Economics);

        public Discipline Discipline { get; set; }
        protected SubjectGroup(int id, string groupCode, Discipline discipline)
            : base(id, groupCode)
        {
            Discipline = discipline;
        }
    }
}
