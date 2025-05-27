using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    public class Question
    {
        public string Text { get; set; }
        public List<AnswerOption> AnswerOptions { get; set; } = new();
    }
}
