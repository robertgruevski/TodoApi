namespace TodoApi
{
	public class Todo // A Model is a class that represents the data in the App.
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public bool IsComplete { get; set; }
	}
}
