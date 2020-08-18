using Journal.Domain.SeedWork;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class SubjectCode
        : Enumeration
    {
        public static SubjectCode AstrophysicsofGalaxies = new SubjectCode(1, "astro-ph.GA", "Astrophysics of Galaxies", SubjectGroup.Astrophysics.Name);
        public static SubjectCode CosmologyandNongalacticAstrophysics = new SubjectCode(2, "astro-ph.CO", "Cosmology and Nongalactic Astrophysics", SubjectGroup.Astrophysics.Name);
        public static SubjectCode EarthandPlanetaryAstrophysics = new SubjectCode(3, "astro-ph.EP", "Earth and Planetary Astrophysics", SubjectGroup.Astrophysics.Name);
        public static SubjectCode HighEnergyAstrophysicalPhenomena = new SubjectCode(4, "astro-ph.HE", "High Energy Astrophysical Phenomena", SubjectGroup.Astrophysics.Name);
        public static SubjectCode InstrumentationandMethodsforAstrophysics = new SubjectCode(5, "astro-ph.IM", "Instrumentation and Methods for Astrophysics", SubjectGroup.Astrophysics.Name);
        public static SubjectCode SolarandStellarAstrophysics = new SubjectCode(6, "astro-ph.SR", "Solar and Stellar Astrophysics", SubjectGroup.Astrophysics.Name);
        public static SubjectCode DisorderedSystemsandNeuralNetworks = new SubjectCode(7, "cond-mat.dis-nn", "Disordered Systems and Neural Networks", SubjectGroup.CondensedMatter.Name);
        public static SubjectCode MaterialsScience = new SubjectCode(8, "cond-mat.mtrl-sci", "Materials Science", SubjectGroup.CondensedMatter.Name);
        public static SubjectCode MesoscaleandNanoscalePhysics = new SubjectCode(9, "cond-mat.mes-hall", "Mesoscale and Nanoscale Physics", SubjectGroup.CondensedMatter.Name);
        public static SubjectCode OtherCondensedMatter = new SubjectCode(10, "cond-mat.other", "Other Condensed Matter", SubjectGroup.CondensedMatter.Name);
        public static SubjectCode QuantumGases = new SubjectCode(11, "cond-mat.quant-gas", "Quantum Gases", SubjectGroup.CondensedMatter.Name);
        public static SubjectCode SoftCondensedMatter = new SubjectCode(12, "cond-mat.soft", "Soft Condensed Matter", SubjectGroup.CondensedMatter.Name);
        public static SubjectCode StatisticalMechanics = new SubjectCode(13, "cond-mat.stat-mech", "Statistical Mechanics", SubjectGroup.CondensedMatter.Name);
        public static SubjectCode StronglyCorrelatedElectrons = new SubjectCode(14, "cond-mat.str-el", "Strongly Correlated Electrons", SubjectGroup.CondensedMatter.Name);
        public static SubjectCode Superconductivity = new SubjectCode(15, "cond-mat.supr-con", "Superconductivity", SubjectGroup.CondensedMatter.Name);
        public static SubjectCode GeneralRelativityandQuantumCosmology = new SubjectCode(16, "gr-qc", "General Relativity and Quantum Cosmology", SubjectGroup.GeneralRelativityQuantumCosmology.Name);
        public static SubjectCode HighEnergyPhysicsExperiment = new SubjectCode(17, "hep-ex", "High Energy Physics - Experiment", SubjectGroup.HighEnergyPhysicsExperiment.Name);
        public static SubjectCode HighEnergyPhysicsLattice = new SubjectCode(18, "hep-lat", "High Energy Physics - Lattice", SubjectGroup.HighEnergyPhysicsLattice.Name);
        public static SubjectCode HighEnergyPhysicsPhenomenology = new SubjectCode(19, "hep-ph", "High Energy Physics - Phenomenology", SubjectGroup.HighEnergyPhysicsPhenomenology.Name);
        public static SubjectCode HighEnergyPhysicsTheory = new SubjectCode(20, "hep-th", "High Energy Physics - Theory", SubjectGroup.HighEnergyPhysicsTheory.Name);
        public static SubjectCode MathematicalPhysics = new SubjectCode(21, "math-ph", "Mathematical Physics", SubjectGroup.MathematicalPhysics.Name);
        public static SubjectCode AdaptationandSelfOrganizingSystems = new SubjectCode(22, "nlin.AO", "Adaptation and Self-Organizing Systems", SubjectGroup.NonlinearSciences.Name);
        public static SubjectCode CellularAutomataandLatticeGases = new SubjectCode(23, "nlin.CG", "Cellular Automata and Lattice Gases", SubjectGroup.NonlinearSciences.Name);
        public static SubjectCode ChaoticDynamics = new SubjectCode(24, "nlin.CD", "Chaotic Dynamics", SubjectGroup.NonlinearSciences.Name);
        public static SubjectCode ExactlySolvableandIntegrableSystems = new SubjectCode(25, "nlin.SI", "Exactly Solvable and Integrable Systems", SubjectGroup.NonlinearSciences.Name);
        public static SubjectCode PatternFormationandSolitons = new SubjectCode(26, "nlin.PS", "Pattern Formation and Solitons", SubjectGroup.NonlinearSciences.Name);
        public static SubjectCode NuclearExperiment = new SubjectCode(27, "nucl-ex", "Nuclear Experiment", SubjectGroup.NuclearExperiment.Name);
        public static SubjectCode NuclearTheory = new SubjectCode(28, "nucl-th", "Nuclear Theory", SubjectGroup.NuclearTheory.Name);
        public static SubjectCode Acceleratorphysics = new SubjectCode(29, "physics.acc-ph", "Acceleratorphysics", SubjectGroup.Physics.Name);
        public static SubjectCode Appliedphysics = new SubjectCode(30, "physics.app-ph", "Appliedphysics", SubjectGroup.Physics.Name);
        public static SubjectCode AtmosphericandOceanicphysics = new SubjectCode(31, "physics.ao-ph", "Atmospheric and Oceanicphysics", SubjectGroup.Physics.Name);
        public static SubjectCode Atomicphysics = new SubjectCode(32, "physics.atom-ph", "Atomicphysics", SubjectGroup.Physics.Name);
        public static SubjectCode AtomicandMolecularClusters = new SubjectCode(33, "physics.atm-clus", "Atomic and Molecular Clusters", SubjectGroup.Physics.Name);
        public static SubjectCode Biologicalphysics = new SubjectCode(34, "physics.bio-ph", "Biologicalphysics", SubjectGroup.Physics.Name);
        public static SubjectCode Chemicalphysics = new SubjectCode(35, "physics.chem-ph", "Chemicalphysics", SubjectGroup.Physics.Name);
        public static SubjectCode Classicalphysics = new SubjectCode(36, "physics.class-ph", "Classicalphysics", SubjectGroup.Physics.Name);
        public static SubjectCode Computationalphysics = new SubjectCode(37, "physics.comp-ph", "Computationalphysics", SubjectGroup.Physics.Name);
        public static SubjectCode DataAnalysis = new SubjectCode(38, "physics.data-an", "Data Analysis", SubjectGroup.Physics.Name);
        public static SubjectCode FluidDynamics = new SubjectCode(39, "physics.flu-dyn", "Fluid Dynamics", SubjectGroup.Physics.Name);
        public static SubjectCode Generalphysics = new SubjectCode(40, "physics.gen-ph", "Generalphysics", SubjectGroup.Physics.Name);
        public static SubjectCode Geophysics = new SubjectCode(41, "physics.geo-ph", "Geophysics", SubjectGroup.Physics.Name);
        public static SubjectCode Historyandphilosophyofphysics = new SubjectCode(42, "physics.hist-ph", "History andphilosophy ofphysics", SubjectGroup.Physics.Name);
        public static SubjectCode InstrumentationandDetectors = new SubjectCode(43, "physics.ins-det", "Instrumentation and Detectors", SubjectGroup.Physics.Name);
        public static SubjectCode Medicalphysics = new SubjectCode(44, "physics.med-ph", "Medicalphysics", SubjectGroup.Physics.Name);
        public static SubjectCode Optics = new SubjectCode(45, "physics.optics", "Optics", SubjectGroup.Physics.Name);
        public static SubjectCode PhysicsEducation = new SubjectCode(46, "physics.ed-ph", "Physics Education", SubjectGroup.Physics.Name);
        public static SubjectCode PhysicsandSociety = new SubjectCode(47, "physics.soc-ph", "Physics and Society", SubjectGroup.Physics.Name);
        public static SubjectCode Plasmaphysics = new SubjectCode(48, "physics.plasm-ph", "Plasmaphysics", SubjectGroup.Physics.Name);
        public static SubjectCode Popularphysics = new SubjectCode(49, "physics.pop-ph", "Popularphysics", SubjectGroup.Physics.Name);
        public static SubjectCode Spacephysics = new SubjectCode(50, "physics.space-ph", "Spacephysics", SubjectGroup.Physics.Name);
        public static SubjectCode QuantumPhysics = new SubjectCode(51, "quant-ph", "Quantum Physics", SubjectGroup.QuantumPhysics.Name);
        public static SubjectCode AlgebraicGeometry = new SubjectCode(52, "math.AG", "Algebraic Geometry", SubjectGroup.Mathematics.Name);
        public static SubjectCode AlgebraicTopology = new SubjectCode(53, "math.AT", "Algebraic Topology", SubjectGroup.Mathematics.Name);
        public static SubjectCode AnalysisofPDEs = new SubjectCode(54, "math.AP", "Analysis of PDEs", SubjectGroup.Mathematics.Name);
        public static SubjectCode CategoryTheory = new SubjectCode(55, "math.CT", "Category Theory", SubjectGroup.Mathematics.Name);
        public static SubjectCode ClassicalAnalysisandODEs = new SubjectCode(56, "math.CA", "Classical Analysis and ODEs", SubjectGroup.Mathematics.Name);
        public static SubjectCode Combinatorics = new SubjectCode(57, "math.CO", "Combinatorics", SubjectGroup.Mathematics.Name);
        public static SubjectCode CommutativeAlgebra = new SubjectCode(58, "math.AC", "Commutative Algebra", SubjectGroup.Mathematics.Name);
        public static SubjectCode ComplexVariables = new SubjectCode(59, "math.CV", "Complex Variables", SubjectGroup.Mathematics.Name);
        public static SubjectCode DifferentialGeometry = new SubjectCode(60, "math.DG", "Differential Geometry", SubjectGroup.Mathematics.Name);
        public static SubjectCode DynamicalSystems = new SubjectCode(61, "math.DS", "Dynamical Systems", SubjectGroup.Mathematics.Name);
        public static SubjectCode FunctionalAnalysis = new SubjectCode(62, "math.FA", "Functional Analysis", SubjectGroup.Mathematics.Name);
        public static SubjectCode Generalmathematics = new SubjectCode(63, "math.GM", "Generalmathematics", SubjectGroup.Mathematics.Name);
        public static SubjectCode GeneralTopology = new SubjectCode(64, "math.GN", "General Topology", SubjectGroup.Mathematics.Name);
        public static SubjectCode GeometricTopology = new SubjectCode(65, "math.GT", "Geometric Topology", SubjectGroup.Mathematics.Name);
        public static SubjectCode GroupTheory = new SubjectCode(66, "math.GR", "Group Theory", SubjectGroup.Mathematics.Name);
        public static SubjectCode HistoryandOverview = new SubjectCode(67, "math.HO", "History and Overview", SubjectGroup.Mathematics.Name);
        public static SubjectCode InformationTheoryInMath = new SubjectCode(68, "math.IT", "Information Theory", SubjectGroup.Mathematics.Name);
        public static SubjectCode KTheoryandHomology = new SubjectCode(69, "math.KT", "K-Theory and Homology", SubjectGroup.Mathematics.Name);
        public static SubjectCode Logic = new SubjectCode(70, "math.LO", "Logic", SubjectGroup.Mathematics.Name);
        public static SubjectCode MathPhysics = new SubjectCode(71, "math.MP", "Mathematical Physics", SubjectGroup.Mathematics.Name);
        public static SubjectCode MetricGeometry = new SubjectCode(72, "math.MG", "Metric Geometry", SubjectGroup.Mathematics.Name);
        public static SubjectCode NumberTheory = new SubjectCode(73, "math.NT", "Number Theory", SubjectGroup.Mathematics.Name);
        public static SubjectCode NumericalAnalysis = new SubjectCode(74, "math.NA", "Numerical Analysis", SubjectGroup.Mathematics.Name);
        public static SubjectCode OperatorAlgebras = new SubjectCode(75, "math.OA", "Operator Algebras", SubjectGroup.Mathematics.Name);
        public static SubjectCode OptimizationandControl = new SubjectCode(76, "math.OC", "Optimization and Control", SubjectGroup.Mathematics.Name);
        public static SubjectCode Probability = new SubjectCode(77, "math.PR", "Probability", SubjectGroup.Mathematics.Name);
        public static SubjectCode QuantumAlgebra = new SubjectCode(78, "math.QA", "Quantum Algebra", SubjectGroup.Mathematics.Name);
        public static SubjectCode RepresentationTheory = new SubjectCode(79, "math.RT", "Representation Theory", SubjectGroup.Mathematics.Name);
        public static SubjectCode RingsandAlgebras = new SubjectCode(80, "math.RA", "Rings and Algebras", SubjectGroup.Mathematics.Name);
        public static SubjectCode SpectralTheory = new SubjectCode(81, "math.SP", "Spectral Theory", SubjectGroup.Mathematics.Name);
        public static SubjectCode StatisticsTheoryInMath = new SubjectCode(82, "math.ST", "Statistics Theory", SubjectGroup.Mathematics.Name);
        public static SubjectCode SymplecticGeometry = new SubjectCode(83, "math.SG", "Symplectic Geometry", SubjectGroup.Mathematics.Name);
        public static SubjectCode ArtificialIntelligence = new SubjectCode(84, "cs.AI", "Artificial Intelligence", SubjectGroup.ComputerScience.Name);
        public static SubjectCode ComputationandLanguage = new SubjectCode(85, "cs.CL", "Computation and Language", SubjectGroup.ComputerScience.Name);
        public static SubjectCode ComputationalComplexity = new SubjectCode(86, "cs.CC", "Computational Complexity", SubjectGroup.ComputerScience.Name);
        public static SubjectCode ComputationalEngineeringFinanceandScience = new SubjectCode(87, "cs.CE", "Computational Engineering Finance and Science", SubjectGroup.ComputerScience.Name);
        public static SubjectCode ComputationalGeometry = new SubjectCode(88, "cs.CG", "Computational Geometry", SubjectGroup.ComputerScience.Name);
        public static SubjectCode ComputerScienceandGameTheory = new SubjectCode(89, "cs.GT", "Computer Science and Game Theory", SubjectGroup.ComputerScience.Name);
        public static SubjectCode ComputerVisionandPatternRecognition = new SubjectCode(90, "cs.CV", "Computer Vision and Pattern Recognition", SubjectGroup.ComputerScience.Name);
        public static SubjectCode ComputersandSociety = new SubjectCode(91, "cs.CY", "Computers and Society", SubjectGroup.ComputerScience.Name);
        public static SubjectCode CryptographyandSecurity = new SubjectCode(92, "cs.CR", "Cryptography and Security", SubjectGroup.ComputerScience.Name);
        public static SubjectCode DataStructuresandAlgorithms = new SubjectCode(93, "cs.DS", "Data Structures and Algorithms", SubjectGroup.ComputerScience.Name);
        public static SubjectCode Databases = new SubjectCode(94, "cs.DB", "Databases", SubjectGroup.ComputerScience.Name);
        public static SubjectCode DigitalLibraries = new SubjectCode(95, "cs.DL", "Digital Libraries", SubjectGroup.ComputerScience.Name);
        public static SubjectCode DiscreteMathematics = new SubjectCode(96, "cs.DM", "Discrete Mathematics", SubjectGroup.ComputerScience.Name);
        public static SubjectCode DistributedParallelandClusterComputing = new SubjectCode(97, "cs.DC", "Distributed Parallel and Cluster Computing", SubjectGroup.ComputerScience.Name);
        public static SubjectCode EmergingTechnologies = new SubjectCode(98, "cs.ET", "Emerging Technologies", SubjectGroup.ComputerScience.Name);
        public static SubjectCode FormalLanguagesandAutomataTheory = new SubjectCode(99, "cs.FL", "Formal Languages and Automata Theory", SubjectGroup.ComputerScience.Name);
        public static SubjectCode GeneralLiterature = new SubjectCode(100, "cs.GL", "General Literature", SubjectGroup.ComputerScience.Name);
        public static SubjectCode Graphics = new SubjectCode(101, "cs.GR", "Graphics", SubjectGroup.ComputerScience.Name);
        public static SubjectCode HardwareArchitecture = new SubjectCode(102, "cs.AR", "Hardware Architecture", SubjectGroup.ComputerScience.Name);
        public static SubjectCode HumanComputerInteraction = new SubjectCode(103, "cs.HC", "Human-Computer Interaction", SubjectGroup.ComputerScience.Name);
        public static SubjectCode InformationRetrieval = new SubjectCode(104, "cs.IR", "Information Retrieval", SubjectGroup.ComputerScience.Name);
        public static SubjectCode InformationTheory = new SubjectCode(105, "cs.IT", "Information Theory", SubjectGroup.ComputerScience.Name);
        public static SubjectCode LogicinComputerScience = new SubjectCode(106, "cs.LO", "Logic in Computer Science", SubjectGroup.ComputerScience.Name);
        public static SubjectCode MachineLearning = new SubjectCode(107, "cs.LG", "Machine Learning", SubjectGroup.ComputerScience.Name);
        public static SubjectCode MathematicalSoftware = new SubjectCode(108, "cs.MS", "Mathematical Software", SubjectGroup.ComputerScience.Name);
        public static SubjectCode MultiagentSystems = new SubjectCode(109, "cs.MA", "Multiagent Systems", SubjectGroup.ComputerScience.Name);
        public static SubjectCode Multimedia = new SubjectCode(110, "cs.MM", "Multimedia", SubjectGroup.ComputerScience.Name);
        public static SubjectCode NetworkingandInternetArchitecture = new SubjectCode(111, "cs.NI", "Networking and Internet Architecture", SubjectGroup.ComputerScience.Name);
        public static SubjectCode NeuralandEvolutionaryComputing = new SubjectCode(112, "cs.NE", "Neural and Evolutionary Computing", SubjectGroup.ComputerScience.Name);
        public static SubjectCode NumericalAnalysisInCs = new SubjectCode(113, "cs.NA", "Numerical Analysis", SubjectGroup.ComputerScience.Name);
        public static SubjectCode OperatingSystems = new SubjectCode(114, "cs.OS", "Operating Systems", SubjectGroup.ComputerScience.Name);
        public static SubjectCode OtherComputerScience = new SubjectCode(115, "cs.OH", "Other Computer Science", SubjectGroup.ComputerScience.Name);
        public static SubjectCode Performance = new SubjectCode(116, "cs.PF", "Performance", SubjectGroup.ComputerScience.Name);
        public static SubjectCode ProgrammingLanguages = new SubjectCode(117, "cs.PL", "Programming Languages", SubjectGroup.ComputerScience.Name);
        public static SubjectCode Robotics = new SubjectCode(118, "cs.RO", "Robotics", SubjectGroup.ComputerScience.Name);
        public static SubjectCode SocialandInformationNetworks = new SubjectCode(119, "cs.SI", "Social and Information Networks", SubjectGroup.ComputerScience.Name);
        public static SubjectCode SoftwareEngineering = new SubjectCode(120, "cs.SE", "Software Engineering", SubjectGroup.ComputerScience.Name);
        public static SubjectCode Sound = new SubjectCode(121, "cs.SD", "Sound", SubjectGroup.ComputerScience.Name);
        public static SubjectCode SymbolicComputation = new SubjectCode(122, "cs.SC", "Symbolic Computation", SubjectGroup.ComputerScience.Name);
        public static SubjectCode SystemsandControl = new SubjectCode(123, "cs.SY", "Systems and Control", SubjectGroup.ComputerScience.Name);
        public static SubjectCode Biomolecules = new SubjectCode(124, "q-bio.BM", "Biomolecules", SubjectGroup.QuantitativeBiology.Name);
        public static SubjectCode CellBehavior = new SubjectCode(125, "q-bio.CB", "Cell Behavior", SubjectGroup.QuantitativeBiology.Name);
        public static SubjectCode Genomics = new SubjectCode(126, "q-bio.GN", "Genomics", SubjectGroup.QuantitativeBiology.Name);
        public static SubjectCode MolecularNetworks = new SubjectCode(127, "q-bio.MN", "Molecular Networks", SubjectGroup.QuantitativeBiology.Name);
        public static SubjectCode NeuronsandCognition = new SubjectCode(128, "q-bio.NC", "Neurons and Cognition", SubjectGroup.QuantitativeBiology.Name);
        public static SubjectCode OtherQuantitativeBiology = new SubjectCode(129, "q-bio.OT", "Other Quantitative Biology", SubjectGroup.QuantitativeBiology.Name);
        public static SubjectCode PopulationsandEvolution = new SubjectCode(130, "q-bio.PE", "Populations and Evolution", SubjectGroup.QuantitativeBiology.Name);
        public static SubjectCode QuantitativeMethods = new SubjectCode(131, "q-bio.QM", "Quantitative Methods", SubjectGroup.QuantitativeBiology.Name);
        public static SubjectCode SubcellularProcesses = new SubjectCode(132, "q-bio.SC", "Subcellular Processes", SubjectGroup.QuantitativeBiology.Name);
        public static SubjectCode TissuesandOrgans = new SubjectCode(133, "q-bio.TO", "Tissues and Organs", SubjectGroup.QuantitativeBiology.Name);
        public static SubjectCode ComputationalFinance = new SubjectCode(134, "q-fin.CP", "Computational Finance", SubjectGroup.QuantitativeFinance.Name);
        public static SubjectCode Economics = new SubjectCode(135, "q-fin.EC", "Economics", SubjectGroup.QuantitativeFinance.Name);
        public static SubjectCode GeneralFinance = new SubjectCode(136, "q-fin.GN", "General Finance", SubjectGroup.QuantitativeFinance.Name);
        public static SubjectCode MathematicalFinance = new SubjectCode(137, "q-fin.MF", "Mathematical Finance", SubjectGroup.QuantitativeFinance.Name);
        public static SubjectCode PortfolioManagement = new SubjectCode(138, "q-fin.PM", "Portfolio Management", SubjectGroup.QuantitativeFinance.Name);
        public static SubjectCode PricingofSecurities = new SubjectCode(139, "q-fin.PR", "Pricing of Securities", SubjectGroup.QuantitativeFinance.Name);
        public static SubjectCode RiskManagement = new SubjectCode(140, "q-fin.RM", "Risk Management", SubjectGroup.QuantitativeFinance.Name);
        public static SubjectCode StatisticalFinance = new SubjectCode(141, "q-fin.ST", "Statistical Finance", SubjectGroup.QuantitativeFinance.Name);
        public static SubjectCode TradingandMarketMicrostructure = new SubjectCode(142, "q-fin.TR", "Trading and Market Microstructure", SubjectGroup.QuantitativeFinance.Name);
        public static SubjectCode Applications = new SubjectCode(143, "stat.AP", "Applications", SubjectGroup.Statistics.Name);
        public static SubjectCode Computation = new SubjectCode(144, "stat.CO", "Computation", SubjectGroup.Statistics.Name);
        public static SubjectCode StatMachineLearning = new SubjectCode(145, "stat.ML", "Machine Learning", SubjectGroup.Statistics.Name);
        public static SubjectCode Methodology = new SubjectCode(146, "stat.ME", "Methodology", SubjectGroup.Statistics.Name);
        public static SubjectCode OtherStatistics = new SubjectCode(147, "stat.OT", "Other Statistics", SubjectGroup.Statistics.Name);
        public static SubjectCode StatisticsTheory = new SubjectCode(148, "stat.TH", "Statistics Theory", SubjectGroup.Statistics.Name);
        public static SubjectCode AudioandSpeechProcessing = new SubjectCode(149, "eess.AS", "Audio and Speech Processing", SubjectGroup.ElectricalEngineeringSystemsScience.Name);
        public static SubjectCode ImageandVideoProcessing = new SubjectCode(150, "eess.IV", "Image and Video Processing", SubjectGroup.ElectricalEngineeringSystemsScience.Name);
        public static SubjectCode SignalProcessing = new SubjectCode(151, "eess.SP", "Signal Processing", SubjectGroup.ElectricalEngineeringSystemsScience.Name);
        public static SubjectCode EessSystemsControl = new SubjectCode(152, "eess.SY", "Systems and Control", SubjectGroup.ElectricalEngineeringSystemsScience.Name);
        public static SubjectCode Econometrics = new SubjectCode(153, "econ.EM", "Econometrics", SubjectGroup.Economics.Name);
        public static SubjectCode GeneralEconomics = new SubjectCode(154, "econ.GN", "General Economics", SubjectGroup.Economics.Name);
        public static SubjectCode TheoreticalEconomics = new SubjectCode(155, "econ.TH", "Theoretical Economics", SubjectGroup.Economics.Name);
        
        [NotMapped]
        public string SubjectGroupCode { get; private set; }
        public string Description { get; private set; }
        protected SubjectCode(int id, string name, string description, string subjectGroupCode)
            : base(id, name)
        {
            SubjectGroupCode = subjectGroupCode;
            Description = description;
        }

        public static SubjectCode FindByCode(string code)
        {
            return GetAll<SubjectCode>().SingleOrDefault(s => s.Name == code.Trim());
        } 
    }
}
