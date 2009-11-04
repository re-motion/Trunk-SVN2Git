// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Oracle.DataAccess.Client;
using System.IO;
using System.Text;
using System.Data;

namespace Remotion.Data.DomainObjects.Oracle.ExecSql
{
  class Program
  {
    static int Main (string[] args)
    {
      if (args.Length != 2)
      {
        Console.Error.WriteLine ("syntax: execsql logon file");
        Console.Error.WriteLine ("  logon is as in sqlplus.exe: <username>[/<password>][@<connect_identifier>] ");
        Console.Error.WriteLine ("  e.g. ich/pwd@localhost:1521/orcl");
        return 1;
      }

      string logon = args[0];
      string sqlFile = args[1];

      Regex logonregex = new Regex (@"(?<user>[^/@]+)"
                                  + @"(/(?<password>.+))?"
                                  + @"@(?<host>.+)" 
                                  + @":(?<port>[0-9]+)"
                                  + @"/(?<service>.+)");

      Match match = logonregex.Match (logon);

      string password = match.Groups["password"].ToString();

      if (password == "")
      {
        Console.Write ("Password: ");
        password = Console.ReadLine ();
      }

      string connstr = string.Format ("Data Source= (DESCRIPTION= (ADDRESS_LIST= (ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})) ) "
                                      + "(CONNECT_DATA= (SERVER=DEDICATED) (SERVICE_NAME={2}) ) ); User Id={3}; Password={4};",
                                      match.Groups["host"], match.Groups["port"], match.Groups["service"], match.Groups["user"], password);

      OracleConnection conn = new OracleConnection (connstr);
      conn.Open();

      string[] keywords = {"CREATE", "ALTER", "DECLARE", "DROP", "INSERT"};

      TextReader reader = new StreamReader (sqlFile);
      StringBuilder sb = new StringBuilder (2048);
      for (string line = reader.ReadLine (); line != null; line = reader.ReadLine())
      {
        if (StartsWithAny (line, keywords))
        {
          Execute (sb.ToString(), conn);
          sb.Length = 0;
        }

        if (! line.TrimStart (' ').StartsWith ("--"))
          sb.Append (line + "\n");
      }
      Execute (sb.ToString(), conn);

      conn.Close();

      Console.WriteLine ("Done.");

      return 0;
    }

    private static void Execute (string cmd, OracleConnection conn)
    {
      cmd = cmd.TrimEnd ('\n', '\r');
      if (! cmd.EndsWith ("END;"))
        cmd = cmd.TrimEnd (';');

      if (cmd.Length == 0)
        return;
      try
      {
        OracleCommand command = new OracleCommand (cmd, conn);
        command.CommandType = CommandType.Text;
        command.ExecuteNonQuery();
      }
      catch (OracleException e)
      {
        if (cmd.StartsWith ("DROP") && e.Errors.Count == 1 && e.Errors[0].Number == 942) // table or view does not exist)
          return;

        Console.Error.WriteLine ("\n-------------------------------\nError executing the following command:\n");
        Console.Error.WriteLine (cmd);
        Console.Error.WriteLine ("\n{0}", e.Message);
      }
    }

    private static bool StartsWithAny (string s, string[] words)
    {
      for (int i = 0; i < words.Length; ++i)
      {
        if (s.StartsWith (words[i]))
          return true;
      }

      return false;
    }
  }
}
