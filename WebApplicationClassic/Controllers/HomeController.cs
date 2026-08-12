using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplicationClassic.Controllers
{
	// SCENARIO "aspnet-classic-basics" (.NET Framework 4.8 / ASP.NET Classic - "basics only")
	//   README -> Basics -> Core actions, Breakpoints 1.1, Stepping 1/3/5,
	//   Threads & Variables 2-4, Evaluation, More -> Console.
	// SETUP: run WebApplicationClassic under IIS Express with the debugger.
	// BREAKPOINT: on the first Console.WriteLine in Index() below.
	// ACTION: open the site root in the browser.
	// EXPECTED: the debugger suspends in this .NET Framework controller; the Frames list
	//   shows the System.Web pipeline as the caller.
	// STEP OVER: the five identical WriteLine lines give a stable place to check that each
	//   step advances exactly one line.
	// STEP INTO 'View()': verify the debugger either steps into external code or steps over
	//   it according to the current external-source settings.
	// ALSO: About() and Contact() set ViewBag.Message - put a breakpoint on the
	//   'ViewBag.Message = ...' line and use Set Value to change the string, then verify the
	//   rendered page shows the new text (README -> Basics -> Threads & Variables 4).
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			Console.WriteLine("Hello!");
			Console.WriteLine("Hello!");
			Console.WriteLine("Hello!");
			Console.WriteLine("Hello!");
			Console.WriteLine("Hello!");
			return View();
		}

		public ActionResult About()
		{
			Console.WriteLine("Hello!");
			Console.WriteLine("Hello!");
			Console.WriteLine("Hello!");
			Console.WriteLine("Hello!");
			Console.WriteLine("Hello!");
			ViewBag.Message = "Your application description page.";
			return View();
		}

		public ActionResult Contact()
		{
			Console.WriteLine("Hello!");
			Console.WriteLine("Hello!");
			Console.WriteLine("Hello!");
			Console.WriteLine("Hello!");
			Console.WriteLine("Hello!");
			ViewBag.Message = "Your contact page.";
			return View();
		}
	}
}