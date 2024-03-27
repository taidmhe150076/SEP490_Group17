using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO
{
    public class FeedbackResult
    {
        public string Question { get; set; }
        public double POS { get; set; }
        public double NEU { get; set; }
        public double NEG { get; set; }

        public string getResult()
        {
            double max = this.POS;
            string sentiment = "Positive";
            if (this.NEU > max)
            {
                sentiment = "Neutral";
            }
            if (this.NEG > max)
            {
                sentiment = "Negative";
            }
            return sentiment;
        }

        public int startRating()
        {
            string rating = "";
            double max = this.POS;
            int notch = 1;
            if (this.NEG > max)
            {
                max = this.NEG;
                notch = -1;
            }
            if (this.NEU > max)
            {
                max = this.NEU;
                notch = 0;
            }
            int rating_point = ratingPoint(max, notch);

            return rating_point;
        }

        private int ratingPoint(double max, int notch)
        {
            int default_rating = 3;

            if (max >= 0.9 && max <= 1)
            {
                if (notch == 1)
                {
                    return 5;
                }
                else if (notch == -1)
                {
                    return 1;
                }
                else if (notch == 0)
                {
                    return default_rating;
                }
            }
            else if (max >= 0.7 && max < 0.9)
            {
                if (notch == 1)
                {
                    return 4;
                }
                else if (notch == -1)
                {
                    return 2;
                }
                else if (notch == 0)
                {
                    if (this.POS > this.NEG)
                    {
                        return 4;
                    }
                    else if (this.POS < this.NEG)
                    {
                        return 2;
                    }
                    else
                    {
                        return default_rating;
                    }
                }
            }

            else if (max >= 0.5 && max < 0.7)
            {
                if (notch == 1)
                {
                    // max  pos : 0.6
                    if (this.NEG > this.NEU)
                    {
                        return 3;
                    }
                    else if (this.NEG < this.NEU)
                    {
                        return 4;
                    }
                    else
                    {
                        return 4;
                    }
                }
                else if (notch == -1)
                {
                    if (this.POS < this.NEU)
                    {
                        return 2;
                    }
                    else if (this.POS > this.NEU)
                    {
                        return 3;
                    }
                    else
                    {
                        return 3;
                    }
                }
                else if (notch == 0)
                {
                    return default_rating;
                }
            }
            throw new Exception("sadge");
        }
    }
}
