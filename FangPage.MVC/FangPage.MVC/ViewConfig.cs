namespace FangPage.MVC
{
	public class ViewConfig
	{
		private string m_path = string.Empty;

		private string m_include = string.Empty;

		public string path
		{
			get
			{
				return m_path;
			}
			set
			{
				m_path = value;
			}
		}

		public string include
		{
			get
			{
				return m_include;
			}
			set
			{
				m_include = value;
			}
		}
	}
}
