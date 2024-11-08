using MySql.Data.MySqlClient;
using System.Text;

namespace MySqlCrud
{
    internal class DBConnection
    {
        private const bool DEBUG_MODE = false;

        private List<MySqlConnection> _connections;

        private const string DEFAULT_CONNECTION_STRING =
            "server=127.0.0.1;" +
            "database=TestDatabase;" +
            "uid=root";

        #region Singleton Pattern
        private static DBConnection _instance;
        public static DBConnection Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new DBConnection();
                return _instance;
            }
            private set { }
        }
        private DBConnection()
        {
            _connections = new List<MySqlConnection>();
        }
        #endregion

        private MySqlConnection? OpenConnection(string connectionString)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            // Attempt to open connection to DB
            try
            {
                connection.Open();
                if (connection.Ping())
                    return connection;

                if (DEBUG_MODE)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Database did not respond to ping.");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Connection String Attempted: " + connectionString);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                return null;
            }
            // Log error to screen
            catch (Exception e)
            {
                if (DEBUG_MODE)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: " + e.Message);
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Connection String Attempted: " + connectionString);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                return null;
            }
        }

        /// <summary>
        /// Create new entries in a given table
        /// </summary>
        /// <param name="tableName">Name of table to create data in</param>
        /// <param name="columns">Columns of data provided</param>
        /// <param name="values">Values of data provided</param>
        /// <returns>True if successful otherwise false</returns>
        public bool Create(string tableName, string[] columns, string[,]values)
        {
            // Invalid if length of columns array does not match number of columns in values array
            if (columns.Length != values.GetLength(1))
            {
                if (DEBUG_MODE)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Mismatch between provided number of columns in columns array and values array.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                return false;
            }
                

            // Create SQL Query
            StringBuilder sqlBuilder = new StringBuilder();

            // Table to insert into
            sqlBuilder.Append("INSERT INTO " + tableName + "(");

            // Specified columns
            for(int i = 0; i < columns.Length; i++) {
                if (i != 0)
                    sqlBuilder.Append(", ");
                sqlBuilder.Append(columns[i] + "");
            }

            // Values for Columns
            sqlBuilder.Append(") VALUES");
            // Iterate through rows
            for (int row = 0; row < values.GetLength(0); row++)
            {
                if (row != 0)
                    sqlBuilder.Append(",");
                sqlBuilder.Append(" (");
                // Iterate through a row's columns
                for (int column = 0; column < values.GetLength(1); column++)
                {
                    if (column != 0)
                        sqlBuilder.Append(", ");
                    sqlBuilder.Append("'" + values[row,column] + "'");
                }
                sqlBuilder.Append(")");
            }
            
            // Include delimiter
            sqlBuilder.Append(";");

            // Attempt to run command
            using(MySqlConnection? connection = OpenConnection(DEFAULT_CONNECTION_STRING))
            {
                // Cannot connect
                if (connection == null)
                    return false;
                
                MySqlCommand command = new MySqlCommand(sqlBuilder.ToString(), connection);
                try
                {
                    command.ExecuteNonQuery();
                }
                // Log error and attempted query to console
                catch (Exception e)
                {
                    if (DEBUG_MODE)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error: " + e.Message);
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Query Attempted: " + sqlBuilder.ToString());
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
            return true;
        }
    }
}