using Journal.Domain.SeedWork;
using System.Linq;

namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class SubjectCode
        : Enumeration
    {
        public static SubjectCode AstrophysicsofGalaxies = new SubjectCode(1, "astro-ph.GA", SubjectGroup.Astrophysics);
        public static SubjectCode CosmologyandNongalacticAstrophysics = new SubjectCode(2, "astro-ph.CO", SubjectGroup.Astrophysics);
        public static SubjectCode EarthandPlanetaryAstrophysics = new SubjectCode(3, "astro-ph.EP", SubjectGroup.Astrophysics);
        public static SubjectCode HighEnergyAstrophysicalPhenomena = new SubjectCode(4, "astro-ph.HE", SubjectGroup.Astrophysics);
        public static SubjectCode InstrumentationandMethodsforAstrophysics = new SubjectCode(5, "astro-ph.IM", SubjectGroup.Astrophysics);
        public static SubjectCode SolarandStellarAstrophysics = new SubjectCode(6, "astro-ph.SR", SubjectGroup.Astrophysics);
        public static SubjectCode DisorderedSystemsandNeuralNetworks = new SubjectCode(1, "cond-mat.dis-nn", SubjectGroup.CondensedMatter);
        public static SubjectCode MaterialsScience = new SubjectCode(1, "cond-mat.mtrl-sci", SubjectGroup.CondensedMatter);
        public static SubjectCode MesoscaleandNanoscalePhysics = new SubjectCode(1, "cond-mat.mes-hall", SubjectGroup.CondensedMatter);
        public static SubjectCode OtherCondensedMatter = new SubjectCode(1, "cond-mat.other", SubjectGroup.CondensedMatter);
        public static SubjectCode QuantumGases = new SubjectCode(1, "cond-mat.quant-gas", SubjectGroup.CondensedMatter);
        public static SubjectCode SoftCondensedMatter = new SubjectCode(1, "cond-mat.soft", SubjectGroup.CondensedMatter);
        public static SubjectCode StatisticalMechanics = new SubjectCode(1, "cond-mat.stat-mech", SubjectGroup.CondensedMatter);
        public static SubjectCode StronglyCorrelatedElectrons = new SubjectCode(1, "cond-mat.str-el", SubjectGroup.CondensedMatter);
        public static SubjectCode Superconductivity = new SubjectCode(1, "cond-mat.supr-con", SubjectGroup.CondensedMatter);
        public static SubjectCode GeneralRelativityandQuantumCosmology = new SubjectCode(1, "gr-qc", SubjectGroup.GeneralRelativityQuantumCosmology);
        public static SubjectCode HighEnergyPhysicsExperiment = new SubjectCode(1, "hep-ex", SubjectGroup.HighEnergyPhysicsExperiment);
        public static SubjectCode HighEnergyPhysicsLattice = new SubjectCode(1, "hep-lat", SubjectGroup.HighEnergyPhysicsLattice);
        public static SubjectCode HighEnergyPhysicsPhenomenology = new SubjectCode(1, "hep-ph", SubjectGroup.HighEnergyPhysicsPhenomenology);
        public static SubjectCode HighEnergyPhysicsTheory = new SubjectCode(1, "hep-th", SubjectGroup.HighEnergyPhysicsTheory);
        public static SubjectCode MathematicalPhysics = new SubjectCode(1, "math-ph", SubjectGroup.MathematicalPhysics);
        public static SubjectCode AdaptationandSelfOrganizingSystems = new SubjectCode(1, "nlin.AO", SubjectGroup.NonlinearSciences);
        public static SubjectCode CellularAutomataandLatticeGases = new SubjectCode(1, "nlin.CG", SubjectGroup.NonlinearSciences);
        public static SubjectCode ChaoticDynamics = new SubjectCode(1, "nlin.CD", SubjectGroup.NonlinearSciences);
        public static SubjectCode ExactlySolvableandIntegrableSystems = new SubjectCode(1, "nlin.SI", SubjectGroup.NonlinearSciences);
        public static SubjectCode PatternFormationandSolitons = new SubjectCode(1, "nlin.PS", SubjectGroup.NonlinearSciences);
        public static SubjectCode NuclearExperiment = new SubjectCode(1, "nucl-ex", SubjectGroup.NuclearExperiment);
        public static SubjectCode NuclearTheory = new SubjectCode(1, "nucl-th", SubjectGroup.NuclearTheory);
        public static SubjectCode Acceleratorphysics = new SubjectCode(1, "physics.acc-ph", SubjectGroup.Physics);
        public static SubjectCode Appliedphysics = new SubjectCode(1, "physics.app-ph", SubjectGroup.Physics);
        public static SubjectCode AtmosphericandOceanicphysics = new SubjectCode(1, "physics.ao-ph", SubjectGroup.Physics);
        public static SubjectCode Atomicphysics = new SubjectCode(1, "physics.atom-ph", SubjectGroup.Physics);
        public static SubjectCode AtomicandMolecularClusters = new SubjectCode(1, "physics.atm-clus", SubjectGroup.Physics);
        public static SubjectCode Biologicalphysics = new SubjectCode(1, "physics.bio-ph", SubjectGroup.Physics);
        public static SubjectCode Chemicalphysics = new SubjectCode(1, "physics.chem-ph", SubjectGroup.Physics);
        public static SubjectCode Classicalphysics = new SubjectCode(1, "physics.class-ph", SubjectGroup.Physics);
        public static SubjectCode Computationalphysics = new SubjectCode(1, "physics.comp-ph", SubjectGroup.Physics);
        public static SubjectCode DataAnalysis = new SubjectCode(1, "physics.data-an", SubjectGroup.Physics);
        public static SubjectCode FluidDynamics = new SubjectCode(1, "physics.flu-dyn", SubjectGroup.Physics);
        public static SubjectCode Generalphysics = new SubjectCode(1, "physics.gen-ph", SubjectGroup.Physics);
        public static SubjectCode Geophysics = new SubjectCode(1, "physics.geo-ph", SubjectGroup.Physics);
        public static SubjectCode Historyandphilosophyofphysics = new SubjectCode(1, "physics.hist-ph", SubjectGroup.Physics);
        public static SubjectCode InstrumentationandDetectors = new SubjectCode(1, "physics.ins-det", SubjectGroup.Physics);
        public static SubjectCode Medicalphysics = new SubjectCode(1, "physics.med-ph", SubjectGroup.Physics);
        public static SubjectCode Optics = new SubjectCode(1, "physics.optics", SubjectGroup.Physics);
        public static SubjectCode PhysicsEducation = new SubjectCode(1, "physics.ed-ph", SubjectGroup.Physics);
        public static SubjectCode PhysicsandSociety = new SubjectCode(1, "physics.soc-ph", SubjectGroup.Physics);
        public static SubjectCode Plasmaphysics = new SubjectCode(1, "physics.plasm-ph", SubjectGroup.Physics);
        public static SubjectCode Popularphysics = new SubjectCode(1, "physics.pop-ph", SubjectGroup.Physics);
        public static SubjectCode Spacephysics = new SubjectCode(1, "physics.space-ph", SubjectGroup.Physics);
        public static SubjectCode QuantumPhysics = new SubjectCode(1, "quant-ph", SubjectGroup.QuantumPhysics);
        public static SubjectCode AlgebraicGeometry = new SubjectCode(1, "math.AG", SubjectGroup.Mathematics);
        public static SubjectCode AlgebraicTopology = new SubjectCode(1, "math.AT", SubjectGroup.Mathematics);
        public static SubjectCode AnalysisofPDEs = new SubjectCode(1, "math.AP", SubjectGroup.Mathematics);
        public static SubjectCode CategoryTheory = new SubjectCode(1, "math.CT", SubjectGroup.Mathematics);
        public static SubjectCode ClassicalAnalysisandODEs = new SubjectCode(1, "math.CA", SubjectGroup.Mathematics);
        public static SubjectCode Combinatorics = new SubjectCode(1, "math.CO", SubjectGroup.Mathematics);
        public static SubjectCode CommutativeAlgebra = new SubjectCode(1, "math.AC", SubjectGroup.Mathematics);
        public static SubjectCode ComplexVariables = new SubjectCode(1, "math.CV", SubjectGroup.Mathematics);
        public static SubjectCode DifferentialGeometry = new SubjectCode(1, "math.DG", SubjectGroup.Mathematics);
        public static SubjectCode DynamicalSystems = new SubjectCode(1, "math.DS", SubjectGroup.Mathematics);
        public static SubjectCode FunctionalAnalysis = new SubjectCode(1, "math.FA", SubjectGroup.Mathematics);
        public static SubjectCode Generalmathematics = new SubjectCode(1, "math.GM", SubjectGroup.Mathematics);
        public static SubjectCode GeneralTopology = new SubjectCode(1, "math.GN", SubjectGroup.Mathematics);
        public static SubjectCode GeometricTopology = new SubjectCode(1, "math.GT", SubjectGroup.Mathematics);
        public static SubjectCode GroupTheory = new SubjectCode(1, "math.GR", SubjectGroup.Mathematics);
        public static SubjectCode HistoryandOverview = new SubjectCode(1, "math.HO", SubjectGroup.Mathematics);
        public static SubjectCode InformationTheoryInMath = new SubjectCode(1, "math.IT", SubjectGroup.Mathematics);
        public static SubjectCode KTheoryandHomology = new SubjectCode(1, "math.KT", SubjectGroup.Mathematics);
        public static SubjectCode Logic = new SubjectCode(1, "math.LO", SubjectGroup.Mathematics);
        public static SubjectCode MathPhysics = new SubjectCode(1, "math.MP", SubjectGroup.Mathematics);
        public static SubjectCode MetricGeometry = new SubjectCode(1, "math.MG", SubjectGroup.Mathematics);
        public static SubjectCode NumberTheory = new SubjectCode(1, "math.NT", SubjectGroup.Mathematics);
        public static SubjectCode NumericalAnalysis = new SubjectCode(1, "math.NA", SubjectGroup.Mathematics);
        public static SubjectCode OperatorAlgebras = new SubjectCode(1, "math.OA", SubjectGroup.Mathematics);
        public static SubjectCode OptimizationandControl = new SubjectCode(1, "math.OC", SubjectGroup.Mathematics);
        public static SubjectCode Probability = new SubjectCode(1, "math.PR", SubjectGroup.Mathematics);
        public static SubjectCode QuantumAlgebra = new SubjectCode(1, "math.QA", SubjectGroup.Mathematics);
        public static SubjectCode RepresentationTheory = new SubjectCode(1, "math.RT", SubjectGroup.Mathematics);
        public static SubjectCode RingsandAlgebras = new SubjectCode(1, "math.RA", SubjectGroup.Mathematics);
        public static SubjectCode SpectralTheory = new SubjectCode(1, "math.SP", SubjectGroup.Mathematics);
        public static SubjectCode StatisticsTheoryInMath = new SubjectCode(1, "math.ST", SubjectGroup.Mathematics);
        public static SubjectCode SymplecticGeometry = new SubjectCode(1, "math.SG", SubjectGroup.Mathematics);
        public static SubjectCode ArtificialIntelligence = new SubjectCode(1, "cs.AI", SubjectGroup.ComputerScience);
        public static SubjectCode ComputationandLanguage = new SubjectCode(1, "cs.CL", SubjectGroup.ComputerScience);
        public static SubjectCode ComputationalComplexity = new SubjectCode(1, "cs.CC", SubjectGroup.ComputerScience);
        public static SubjectCode ComputationalEngineeringFinanceandScience = new SubjectCode(1, "cs.CE", SubjectGroup.ComputerScience);
        public static SubjectCode ComputationalGeometry = new SubjectCode(1, "cs.CG", SubjectGroup.ComputerScience);
        public static SubjectCode ComputerScienceandGameTheory = new SubjectCode(1, "cs.GT", SubjectGroup.ComputerScience);
        public static SubjectCode ComputerVisionandPatternRecognition = new SubjectCode(1, "cs.CV", SubjectGroup.ComputerScience);
        public static SubjectCode ComputersandSociety = new SubjectCode(1, "cs.CY", SubjectGroup.ComputerScience);
        public static SubjectCode CryptographyandSecurity = new SubjectCode(1, "cs.CR", SubjectGroup.ComputerScience);
        public static SubjectCode DataStructuresandAlgorithms = new SubjectCode(1, "cs.DS", SubjectGroup.ComputerScience);
        public static SubjectCode Databases = new SubjectCode(1, "cs.DB", SubjectGroup.ComputerScience);
        public static SubjectCode DigitalLibraries = new SubjectCode(1, "cs.DL", SubjectGroup.ComputerScience);
        public static SubjectCode DiscreteMathematics = new SubjectCode(1, "cs.DM", SubjectGroup.ComputerScience);
        public static SubjectCode DistributedParallelandClusterComputing = new SubjectCode(1, "cs.DC", SubjectGroup.ComputerScience);
        public static SubjectCode EmergingTechnologies = new SubjectCode(1, "cs.ET", SubjectGroup.ComputerScience);
        public static SubjectCode FormalLanguagesandAutomataTheory = new SubjectCode(1, "cs.FL", SubjectGroup.ComputerScience);
        public static SubjectCode GeneralLiterature = new SubjectCode(1, "cs.GL", SubjectGroup.ComputerScience);
        public static SubjectCode Graphics = new SubjectCode(1, "cs.GR", SubjectGroup.ComputerScience);
        public static SubjectCode HardwareArchitecture = new SubjectCode(1, "cs.AR", SubjectGroup.ComputerScience);
        public static SubjectCode HumanComputerInteraction = new SubjectCode(1, "cs.HC", SubjectGroup.ComputerScience);
        public static SubjectCode InformationRetrieval = new SubjectCode(1, "cs.IR", SubjectGroup.ComputerScience);
        public static SubjectCode InformationTheory = new SubjectCode(1, "cs.IT", SubjectGroup.ComputerScience);
        public static SubjectCode LogicinComputerScience = new SubjectCode(1, "cs.LO", SubjectGroup.ComputerScience);
        public static SubjectCode MachineLearning = new SubjectCode(1, "cs.LG", SubjectGroup.ComputerScience);
        public static SubjectCode MathematicalSoftware = new SubjectCode(1, "cs.MS", SubjectGroup.ComputerScience);
        public static SubjectCode MultiagentSystems = new SubjectCode(1, "cs.MA", SubjectGroup.ComputerScience);
        public static SubjectCode Multimedia = new SubjectCode(1, "cs.MM", SubjectGroup.ComputerScience);
        public static SubjectCode NetworkingandInternetArchitecture = new SubjectCode(1, "cs.NI", SubjectGroup.ComputerScience);
        public static SubjectCode NeuralandEvolutionaryComputing = new SubjectCode(1, "cs.NE", SubjectGroup.ComputerScience);
        public static SubjectCode NumericalAnalysisInCs = new SubjectCode(1, "cs.NA", SubjectGroup.ComputerScience);
        public static SubjectCode OperatingSystems = new SubjectCode(1, "cs.OS", SubjectGroup.ComputerScience);
        public static SubjectCode OtherComputerScience = new SubjectCode(1, "cs.OH", SubjectGroup.ComputerScience);
        public static SubjectCode Performance = new SubjectCode(1, "cs.PF", SubjectGroup.ComputerScience);
        public static SubjectCode ProgrammingLanguages = new SubjectCode(1, "cs.PL", SubjectGroup.ComputerScience);
        public static SubjectCode Robotics = new SubjectCode(1, "cs.RO", SubjectGroup.ComputerScience);
        public static SubjectCode SocialandInformationNetworks = new SubjectCode(1, "cs.SI", SubjectGroup.ComputerScience);
        public static SubjectCode SoftwareEngineering = new SubjectCode(1, "cs.SE", SubjectGroup.ComputerScience);
        public static SubjectCode Sound = new SubjectCode(1, "cs.SD", SubjectGroup.ComputerScience);
        public static SubjectCode SymbolicComputation = new SubjectCode(1, "cs.SC", SubjectGroup.ComputerScience);
        public static SubjectCode SystemsandControl = new SubjectCode(1, "cs.SY", SubjectGroup.ComputerScience);
        public static SubjectCode Biomolecules = new SubjectCode(1, "q-bio.BM", SubjectGroup.QuantitativeBiology);
        public static SubjectCode CellBehavior = new SubjectCode(1, "q-bio.CB", SubjectGroup.QuantitativeBiology);
        public static SubjectCode Genomics = new SubjectCode(1, "q-bio.GN", SubjectGroup.QuantitativeBiology);
        public static SubjectCode MolecularNetworks = new SubjectCode(1, "q-bio.MN", SubjectGroup.QuantitativeBiology);
        public static SubjectCode NeuronsandCognition = new SubjectCode(1, "q-bio.NC", SubjectGroup.QuantitativeBiology);
        public static SubjectCode OtherQuantitativeBiology = new SubjectCode(1, "q-bio.OT", SubjectGroup.QuantitativeBiology);
        public static SubjectCode PopulationsandEvolution = new SubjectCode(1, "q-bio.PE", SubjectGroup.QuantitativeBiology);
        public static SubjectCode QuantitativeMethods = new SubjectCode(1, "q-bio.QM", SubjectGroup.QuantitativeBiology);
        public static SubjectCode SubcellularProcesses = new SubjectCode(1, "q-bio.SC", SubjectGroup.QuantitativeBiology);
        public static SubjectCode TissuesandOrgans = new SubjectCode(1, "q-bio.TO", SubjectGroup.QuantitativeBiology);
        public static SubjectCode ComputationalFinance = new SubjectCode(1, "q-fin.CP", SubjectGroup.QuantitativeFinance);
        public static SubjectCode Economics = new SubjectCode(1, "q-fin.EC", SubjectGroup.QuantitativeFinance);
        public static SubjectCode GeneralFinance = new SubjectCode(1, "q-fin.GN", SubjectGroup.QuantitativeFinance);
        public static SubjectCode MathematicalFinance = new SubjectCode(1, "q-fin.MF", SubjectGroup.QuantitativeFinance);
        public static SubjectCode PortfolioManagement = new SubjectCode(1, "q-fin.PM", SubjectGroup.QuantitativeFinance);
        public static SubjectCode PricingofSecurities = new SubjectCode(1, "q-fin.PR", SubjectGroup.QuantitativeFinance);
        public static SubjectCode RiskManagement = new SubjectCode(1, "q-fin.RM", SubjectGroup.QuantitativeFinance);
        public static SubjectCode StatisticalFinance = new SubjectCode(1, "q-fin.ST", SubjectGroup.QuantitativeFinance);
        public static SubjectCode TradingandMarketMicrostructure = new SubjectCode(1, "q-fin.TR", SubjectGroup.QuantitativeFinance);
        public static SubjectCode Applications = new SubjectCode(1, "stat.AP", SubjectGroup.Statistics);
        public static SubjectCode Computation = new SubjectCode(1, "stat.CO", SubjectGroup.Statistics);
        public static SubjectCode StatMachineLearning = new SubjectCode(1, "stat.ML", SubjectGroup.Statistics);
        public static SubjectCode Methodology = new SubjectCode(1, "stat.ME", SubjectGroup.Statistics);
        public static SubjectCode OtherStatistics = new SubjectCode(1, "stat.OT", SubjectGroup.Statistics);
        public static SubjectCode StatisticsTheory = new SubjectCode(1, "stat.TH", SubjectGroup.Statistics);
        public static SubjectCode AudioandSpeechProcessing = new SubjectCode(1, "eess.AS", SubjectGroup.ElectricalEngineeringSystemsScience);
        public static SubjectCode ImageandVideoProcessing = new SubjectCode(1, "eess.IV", SubjectGroup.ElectricalEngineeringSystemsScience);
        public static SubjectCode SignalProcessing = new SubjectCode(1, "eess.SP", SubjectGroup.ElectricalEngineeringSystemsScience);
        public static SubjectCode EessSystemsControl = new SubjectCode(1, "eess.SY", SubjectGroup.ElectricalEngineeringSystemsScience);
        public static SubjectCode Econometrics = new SubjectCode(1, "econ.EM", SubjectGroup.Economics);
        public static SubjectCode GeneralEconomics = new SubjectCode(1, "econ.GN", SubjectGroup.Economics);
        public static SubjectCode TheoreticalEconomics = new SubjectCode(1, "econ.TH", SubjectGroup.Economics);

        public SubjectGroup SubjectGroup { get; set; }
        protected SubjectCode(int id, string code, SubjectGroup subjectGroup)
            : base(id, code)
        {
            SubjectGroup = subjectGroup;
        }

        public static SubjectCode FindByCode(string code)
        {
            return GetAll<SubjectCode>().SingleOrDefault(s => s.Name == code.Trim());
        } 
    }
}
