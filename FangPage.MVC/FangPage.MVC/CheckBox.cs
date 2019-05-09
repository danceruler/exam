using System;

namespace FangPage.MVC
{
	public class CheckBox : Attribute
	{
		private bool m_ischeckbox;

		private string m_checkname = string.Empty;

		public string CheckName
		{
			get
			{
				return m_checkname;
			}
			set
			{
				m_checkname = value;
			}
		}

		public bool IsCheckBox
		{
			get
			{
				return m_ischeckbox;
			}
			set
			{
				m_ischeckbox = value;
			}
		}

		public CheckBox()
		{
		}

		public CheckBox(bool isCheckBox)
		{
			m_ischeckbox = isCheckBox;
		}

		public CheckBox(string CheckName)
		{
			m_ischeckbox = true;
			m_checkname = CheckName;
		}
	}
}
