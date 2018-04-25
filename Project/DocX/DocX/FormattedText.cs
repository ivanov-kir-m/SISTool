using System;

namespace Novacode
{
    public class FormattedText: IComparable
    {
        public FormattedText()
        {
        
        }

        public int Index;
        public string Text;
        public Formatting Formatting;

        public int CompareTo(object obj)
        {
            FormattedText other = (FormattedText)obj;
            FormattedText tf = this;

            if (other.Formatting == null || tf.Formatting == null)
                return -1;

            return tf.Formatting.CompareTo(other.Formatting);   
        }
    }
}
