using Journal.Domain.SeedWork;

namespace Journal.Domain.AggregatesModel.SubjectAggregate
{
    public class Subject
         : Entity, IAggregateRoot
    {
        public string Code { get; private set;  }
        public string Name { get; private set;  }
        public string GroupCode { get; private set;  }
        public string GroupName { get; private set;  }
        public string Discipline { get; private set;  }
        public string Description { get; private set;  }

        protected Subject() { }

        public Subject(string subjectCode, string name, string groupName, string groupCode, string discipline,string description)
        {
            Code = subjectCode;
            Name = name;
            GroupName = groupName;
            GroupCode = groupCode;
            Discipline = discipline;
            Description = description;
        }
    }
}
