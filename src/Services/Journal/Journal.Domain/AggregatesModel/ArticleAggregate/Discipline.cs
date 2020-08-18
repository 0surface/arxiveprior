using Journal.Domain.SeedWork;
using System.Linq;

namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class Discipline
        : Enumeration
    {
        public static Discipline Physics = new Discipline(1, "Physics");
        public static Discipline Mathematics = new Discipline(2, "Mathematics");
        public static Discipline ComputerScience = new Discipline(3, "Computer Science");
        public static Discipline QuantitativeBiology = new Discipline(4, "Quantitative Biology");
        public static Discipline QuantitativeFinance = new Discipline(5, "Quantitative Finance");
        public static Discipline EESS = new Discipline(6, "Electrical Engineering and Systems Science");
        public static Discipline Statistics = new Discipline(7, "Statistics");
        public static Discipline Economics = new Discipline(8, "Economics");

        protected Discipline(int id, string name)
             : base(id, name)
        {
        }

        public static Discipline FindByName(string name)
            => GetAll<Discipline>().SingleOrDefault(s => s.Name == name.Trim());
    }
}
