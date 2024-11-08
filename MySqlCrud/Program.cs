namespace MySqlCrud
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] columns = { "FirstName!?", "LastName" };
            string[,] values =
            {
                {"FirstName3", "LastName3"},
                {"FirstName4", "LastName4"}
            };
            DBConnection.Instance.Create("TestTable", columns, values);
        }
    }
}
