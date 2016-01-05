using System;

namespace AssemblyCSharp
{
	[System.Serializable]
	public class VRAction {
		public string id { get; set;}
		public string action { get; set;}
		public string details { get; set;}
		public string time { get; set;}

		public VRAction()
		{
		}
	}
}

