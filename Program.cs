using System.Collections.Generic;
using System.Linq;
using NUnitLite;
using Incapsulation.Failures;

class Program
{
	static void Main(string[] args)
	{
        List<FailureTypes> qwe = new List<FailureTypes>();
        qwe.Add(FailureTypes.Hardware);
        System.Console.WriteLine(qwe[0]);



		new AutoRun().Execute(args);
	}
}
