using System;
using NUnit.Framework;

namespace Battleships
{
	[TestFixture]
	public class UnitTesting
	{
		[Test]
		public void Test_Fullscreen()
		{
			MenuController.full = false;

			Assert.AreEqual(false, MenuController.get_fullscreen);

			MenuController.togglefullscreen();
			Assert.AreEqual(true, MenuController.get_fullscreen);

		}
	}
}

