using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BusTicket
{
    struct Ticket
    {
        public int busStop;         //0-29 //where the custemer gets on the bus
        public int date;            //date when the customer gets on the bus
        public int time;            //time of the customer getting on the bus
        public int ticketID;        //unique if of the customer's pass/ticket //a customer gets on the bus only once during this bus trip
        public string ticketType;   //it can be a pass or a 10 times pass
        public int valid;           //if its a pass date of the experation // if its a 10 times pass the number of tickets, if its 0 then the customer got on the bus without a ticket

        public Ticket(int busStop, int date, int time, int ticketID, string ticketType, int valid)
        {
            this.busStop = busStop;
            this.date = date;
            this.time = time;
            this.ticketID = ticketID;
            this.ticketType = ticketType;
            this.valid = valid;
        }
    }
    class BusTicket
    {

        /*
         * Sample of the utasadat.txt: //the txt records information of one bus strip from start to finish
            
            0 20190326-0700 6572582 RVS 20210101 
            0 20190326-0700 8808290 JGY 7 
            0 20190326-0700 1680423 TAB 20190420 
            12 20190326-0716 3134404 FEB 20190301 
            12 20190326-0716 9529716 JGY 0 
                                       
                                    ticketType Megnevezés 
                                    FEB     adult 
                                    TAB     student (discounted) 
                                    NYB     retured (discounted) 
                                    NYP     age above 65 (free) 
                                    RVS     disabled (free) 
                                    GYK     age under 6 (free) 
                                    JGY     10 times pass
         */
        static List<Ticket> busTrip = new List<Ticket>();

        //Task 1: Read and store the data of the utasadat.txt
        static void Task1()
        {
            StreamReader sr = new StreamReader("utasadat.txt");

            while (!sr.EndOfStream)
            {
                string[] line = sr.ReadLine().Split();
                int busStop = int.Parse(line[0]);
                int date = int.Parse(line[1].Substring(0, 8));
                int time = int.Parse(line[1].Substring(9));
                int ticketID = int.Parse(line[2]);
                string ticketType = line[3];
                int valid = int.Parse(line[4]);

                Ticket item = new Ticket(busStop, date, time, ticketID, ticketType, valid);
                busTrip.Add(item);
            }

            sr.Close();
        }

        //Task 2: Print the number of people that got on the bus
        static void Task2()
        {
            Console.WriteLine("Task 2");
            Console.WriteLine($"\t {busTrip.Count} people got on the bus");
        }

        //Task 3: Print the number of people who got on the bus with an expired pass
        static void Task3()
        {
            Console.WriteLine("Task 3");
            int people = 0;
            foreach (Ticket item in busTrip)
            {
                if (10 < item.valid && item.valid < item.date || item.valid == 0)
                {
                    people++;
                }
            }
            Console.WriteLine($"\t{people} people got on the bus with an expired ticekt.");
        }

        //Task 4: Print the bus stop where most people got on the bus
        static void Task4()
        {
            Console.WriteLine("Task 4");
            int mostPeople = 0;
            int busStop = 0;
            for (int i = 0; i < 30; i++)
            {
                int people = 0;
                foreach (Ticket item in busTrip)
                {
                    if (item.busStop == i)
                    {
                        people++;
                    }
                }
                if (people > mostPeople)
                {
                    mostPeople = people;
                    busStop = i;
                }
            }
            Console.WriteLine($"\tThe most people ({mostPeople}) got on the bus at the bus stop number {busStop} ");
        }

        //Task 5: Print how many people got on the bus with a discounted or a free ticket
        static void Task5()
        {
            Console.WriteLine("Task 5");
            int free = 0;
            int discounted = 0;
            foreach (Ticket item in busTrip)
            {
                if (10 < item.valid && item.valid >= item.date)
                {
                    if (item.ticketType == "TAB" || item.ticketType == "NYB") //Discounted
                        discounted++;
                    if (item.ticketType == "NYP" || item.ticketType == "RVS" || item.ticketType == "GYK") //free
                        free++;
                }
            }
            Console.WriteLine($"\tNumber of people with free tickets: {free}");
            Console.WriteLine($"\tNumber of people with discounted tickets: {discounted}");
        }

        //Task 6: Make a method that takes in two dates as an argument and returns the number of days between the two dates
        static int daysLeft(int ye1, int mo1, int d1, int ye2, int mo2, int d2)
        {
            int[] months = { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 335 };
            ye1 = ye1 - 1;
            ye2 = ye2 - 1;
            int m1 = months[mo1 - 1];
            int y1 = ((ye1 / 4) * 366) + ((ye1 - ye1 / 4) * 365);
            int day1 = y1 + m1 + d1;
            int m2 = months[mo2 - 1];
            int y2 = ((ye2 / 4) * 366) + ((ye2 - ye2 / 4) * 365);
            int day2 = y2 + m2 + d2;
            int daysLeft = day2 - day1;
            return daysLeft;
        }

        //Task 7: List the people whose tickets will expire in 3 days
        static void Task7()
        {
            StreamWriter sw = new StreamWriter("warning.txt");

            foreach (Ticket item in busTrip)
            {
                if (item.ticketType != "JGY") //if not a 10 day pass
                {
                    string date = $"{item.date}";
                    int y1 = int.Parse(date.Substring(0, 4));
                    int m1 = int.Parse(date.Substring(4, 2));
                    int d1 = int.Parse(date.Substring(6));
                    string valid = $"{item.valid}";
                    int y2 = int.Parse(valid.Substring(0, 4));
                    int m2 = int.Parse(valid.Substring(4, 2));
                    int d2 = int.Parse(valid.Substring(6));

                    if (daysLeft(y1, m1, d1, y2, m2, d2) <= 3 && daysLeft(y1, m1, d1, y2, m2, d2) >= 0)
                    {
                        sw.WriteLine($"{item.ticketID} {y2}-{m2}-{d2}");
                    }
                }

            }

            sw.Flush();
            sw.Close();
        }
        static void Main(string[] args)
        {
            Task1();
            Task2();
            Task3();
            Task4();
            Task5();
            Task7();
            Console.ReadKey();
        }
    }
}
