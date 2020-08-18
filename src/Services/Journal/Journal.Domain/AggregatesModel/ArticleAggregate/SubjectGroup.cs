using Journal.Domain.SeedWork;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class SubjectGroup
        : Enumeration
    {
        public static SubjectGroup Astrophysics = new SubjectGroup(1, "astro-ph", Discipline.Physics.Name);
        public static SubjectGroup CondensedMatter = new SubjectGroup(2, "cond-mat", Discipline.Physics.Name);
        public static SubjectGroup GeneralRelativityQuantumCosmology = new SubjectGroup(3, "gr-qc", Discipline.Physics.Name);
        public static SubjectGroup HighEnergyPhysicsExperiment = new SubjectGroup(4, "hep-ex", Discipline.Physics.Name);
        public static SubjectGroup HighEnergyPhysicsLattice = new SubjectGroup(5, "hep-lat", Discipline.Physics.Name);
        public static SubjectGroup HighEnergyPhysicsPhenomenology = new SubjectGroup(6, "hep-ph", Discipline.Physics.Name);
        public static SubjectGroup HighEnergyPhysicsTheory = new SubjectGroup(7, "hep-th", Discipline.Physics.Name);
        public static SubjectGroup MathematicalPhysics = new SubjectGroup(8, "math-ph", Discipline.Physics.Name);
        public static SubjectGroup NonlinearSciences = new SubjectGroup(9, "nlin", Discipline.Physics.Name);
        public static SubjectGroup NuclearExperiment = new SubjectGroup(10, "nucl-ex", Discipline.Physics.Name);
        public static SubjectGroup NuclearTheory = new SubjectGroup(11, "nucl-th", Discipline.Physics.Name);
        public static SubjectGroup Physics = new SubjectGroup(12, "physics", Discipline.Physics.Name);
        public static SubjectGroup QuantumPhysics = new SubjectGroup(13, "quant-ph", Discipline.Physics.Name);
        public static SubjectGroup Mathematics = new SubjectGroup(14, "math", Discipline.Mathematics.Name);
        public static SubjectGroup ComputerScience = new SubjectGroup(15, "cs", Discipline.ComputerScience.Name);
        public static SubjectGroup QuantitativeBiology = new SubjectGroup(16, "q-bio", Discipline.QuantitativeBiology.Name);
        public static SubjectGroup QuantitativeFinance = new SubjectGroup(17, "q-fin", Discipline.QuantitativeFinance.Name);
        public static SubjectGroup Statistics = new SubjectGroup(18, "stat", Discipline.Statistics.Name);
        public static SubjectGroup ElectricalEngineeringSystemsScience = new SubjectGroup(19, "eess", Discipline.EESS.Name);
        public static SubjectGroup Economics = new SubjectGroup(20, "econ", Discipline.Economics.Name);

        [NotMapped]
        public string DisciplineName { get; private set; }
        protected SubjectGroup(int id, string name, string disciplineName)
            : base(id, name)
        {
            DisciplineName = disciplineName;
        }

        public static SubjectGroup FindByCode(string code)
        {
            return GetAll<SubjectGroup>().SingleOrDefault(s => s.Name == code.Trim());
        }
    }
}
