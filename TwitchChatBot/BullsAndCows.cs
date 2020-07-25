using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchChatBot
{
    public class BullsAndCows
    {
        private string userName;
        private string number = string.Empty;
        private int count = 0;
        public bool IsWin { get; private set; }

        public BullsAndCows(string userName)
        {
            this.userName = userName;
            Random rand = new Random();
            while (number.Length < 4)
            {
                char c = Convert.ToChar('0' + (number.Length == 0 ? rand.Next(1, 9) : rand.Next(0, 9)));
                if (!number.Contains(c))
                {
                    number += c;
                }
            }
        }

        public string Guess(string msg)
        {
            IsWin = false;
            count++;
            int cows = 0;
            int bulls = 0;

            string resMsg = string.Empty;
            for (int i = 0; i < number.Length; i++)
            {
                if (msg[i] == number[i])
                {
                    bulls++;
                    resMsg += " ";
                }
                else
                {
                    resMsg += msg[i];
                }
            }
            foreach (var c in resMsg)
            {
                if (number.Contains(c))
                {
                    cows++;
                }
            }
            if (bulls == 4)
            {
                IsWin = true;
                return string.Format("Пользователь: {0} одержал победу! " +
                    "Количество попыток: {1} " +
                    "Загаданное число: {2}", userName, count, number);
            }
            return string.Format("Пользователь: {0} загадал число {1} " +
                "Результат: {2} Быки|{3} Коровы", userName, msg, bulls, cows);
        }
    }
}
