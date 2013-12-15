/////////////////////////////////////////////////
///�ļ���:Template
///��  ��:
///������:����ƽ(Iveely Liu)
///��  ��:liufanping@iveely.com
///��  ֯:Iveely
///��  ��:2012/3/27 20:45:04
///////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iveely.Framework.DataStructure;


namespace Iveely.Framework.Algorithm.AI
{
    /// <summary>
    /// ģ��
    /// </summary>
    public class Template
    {
        /// <summary>
        /// ���ɵ�����
        /// </summary>
        public class Question
        {
            /// <summary>
            /// ��������
            /// </summary>
            public string Doubt { get; set; }

            /// <summary>
            /// ��
            /// </summary>
            public string Answer { get; set; }
        }

        /// <summary>
        /// ģ��ֵ
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// ģ�����ֵ
        /// </summary>
        public Rand Rand { get; set; }

        /// <summary>
        /// ƥ��*���λ��
        /// </summary>
        public SortedList<int> Star { get; set; }

        /// <summary>
        /// �û��ĵڼ������
        /// </summary>
        public int Input { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public Variable SetVariable;

        /// <summary>
        /// ��ȡ�ı���
        /// </summary>
        public Variable GetVariable;

        /// <summary>
        /// ������
        /// </summary>
        public Function Function { get; set; }

        public List<Question> Questions { get; set; }

        /// <summary>
        /// ���췽��
        /// </summary>
        public Template()
        {
            //���ѡ���ʼ��
            Rand = new Rand();
            //���ñ�����ʼ��
            SetVariable = new Variable();
            //��ȡ������ʼ��
            GetVariable = new Variable();
            //�������ϳ�ʼ��
            Function = new Function();
            //*��ƥ�����ʼ��
            Star = new SortedList<int>();
            //��ȡ�û������ʼ��
            Input = -1;
            //��ʼ������
            Questions = new List<Question>();
        }

        /// <summary>
        /// �ظ���Ϣ
        /// </summary>
        /// <returns></returns>
        public string Reply()
        {
            //�ظ���Ϣ
            var result = Value;
            //����Ǵ��м����*�ű�ʶ
            if (Star != null)
            {
                //������ֲ�Ϊ��
                if (SetVariable.Name != "" || SetVariable.Name != null)
                {
                    //����ÿһ��*����
                    for (int i = 0; i < Star.Count; i++)
                    {
                        //�洢������Ŀ
                        User.StoreNum++;
                        //�趨�������
                        Memory.Set(User.UserId + "" + User.StoreNum, SetVariable.Name, AI.Star.List[i]);
                        //break;
                    }
                    //����ֵ
                    //return this.Value;
                }
                //�����Ϣ
                //return this.Value + Smart.Star.List[Star-1];
                //����ÿһ��*����
                //for (int i = 0; i < this.Star.Count; i++)
                //{
                //    result += Smart.Star.List[i];
                //}
            }

            //����ж�ִ̬�к�������
            if (Function != null)
            {
                //��Ҫ���ݵĲ�������
                var parm = new SortedList<string>();
                //�����������ֵȡ����
                var star = Star;
                if (star != null)
                {
                    int[] va = star.ToArray();
                    //����ʵֵȡ����
                    parm.AddRange(va.Select(a => AI.Star.List[a - 1]));
                }
                //ִ�У���ȻҪ�Ѳ������ݹ�ȥ
                return Library.CodeCompiler.Execute(Function.Name, parm.ToArray());
            }
            //��ȡ�洢�ı���ֵ
            if (GetVariable.Name != null)
            {

                //��ȡ����
                result
                    += Memory.Get(User.UserId + "" + User.StoreNum, GetVariable.Name);
            }
            //�������û����������
            if (Input != -1)
            {
                //�����Ϣ
                result += AI.Input.List[Input - 1];
            }
            if (String.IsNullOrEmpty(result))
            {
                //������ķ�ʽ����
                var rnd = new Random();
                //����
                if (Rand != null) return Rand.List[rnd.Next(0, Rand.List.Count)];
            }
            return result;
        }


        public void AddQuestion(Question question)
        {
            this.Questions.Add(question);
        }

        public string BuildQuestion()
        {
            string[] values = AI.Star.List;
            StringBuilder builder = new StringBuilder();
            foreach (var question in Questions)
            {
                string doubt = question.Doubt;
                string answer = question.Answer;
                for (int i = 0; i < values.Count(); i++)
                {
                    doubt = doubt.Replace("[" + i + "]", values[i]);
                    answer = answer.Replace("[" + i + "]", values[i]);
                }

                //doubt = ReplaceStar(doubt, values);
                answer = ReplaceStar(answer, values);

                builder.Append(doubt + "?->System Answer:" + answer);
                builder.AppendLine();
            }
            return builder.ToString();
        }

        private string ReplaceStar(string text, string[] values)
        {
            int indexStar = text.IndexOf("*", System.StringComparison.Ordinal);
            while (indexStar > -1)
            {
                int numberLeftIndex = text.IndexOf("{", StringComparison.Ordinal);
                int numberRightIndex = text.IndexOf("}", StringComparison.Ordinal);
                string[] indexs = text.Substring(numberLeftIndex + 1, numberRightIndex - numberLeftIndex - 1).Split(new[] { ',', '��' });
                int start = int.Parse(indexs[0]);
                int end = start;
                if (indexs.Length > 1)
                {
                    end = int.Parse(indexs[1]);
                }
                text = text.Remove(numberLeftIndex, numberRightIndex - numberLeftIndex + 1);
                while (end >= start)
                {
                    text = text.Remove(indexStar, 1).Insert(indexStar, values[start]);
                    indexStar = text.IndexOf("*", System.StringComparison.Ordinal);
                    start++;
                }

            }
            return text;
        }
    }
}