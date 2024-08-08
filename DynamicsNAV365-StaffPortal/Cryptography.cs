using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DynamicsNAV365_StaffPortal
{
	public class Cryptography
	{
		public static string Hash(string value)
		{
			if (value != null)
			{
				return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(value)));
			}
			else
			{
				return "";
			}

		}
	}
}