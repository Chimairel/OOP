namespace Models
{
    public abstract class Person
    {
        public int Id { get; set; }  
        public string FirstName { get; set; }
        public string LastName { get; set; } 

        public string FullName => $"{FirstName} {LastName}";

        protected Person(int id, string firstName, string lastName)
        {
            Id = id;
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        }

        public abstract void PrintInfo();
    }
}
