using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace collatz_conjecture
{
    class JansLinje
    {
        public Point startPos;
        public Point endPos;
        public SolidColorBrush color;

		public JansLinje(Point startPos, Point endPos, SolidColorBrush color)
		{
			this.startPos = startPos;
			this.endPos = endPos;
			this.color = color;
		}
	}
}
