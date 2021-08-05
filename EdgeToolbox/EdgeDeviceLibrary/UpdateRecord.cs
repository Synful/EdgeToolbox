namespace EdgeDeviceLibrary
{
	public class UpdateRecord
	{
		public long id;

		public string description;

		public string type;

		public string versionString;

		public UpdateRecord(long ID, string Description, string Type, string VersionString)
		{
			id = ID;
			description = Description;
			type = Type;
			versionString = VersionString;
		}
	}
}
