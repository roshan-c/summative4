using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace summative4
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load the student data from the selected file
            StudentData[] studentDataArray = loadData();

            // Clear the console before printing the loaded student data
            Console.Clear();

            // Print the loaded student data to confirm it's there
            PrintLoadedData(studentDataArray);

            // Output the data to a text file
            OutputDataToFile(studentDataArray, "output.txt");
        }

        static StudentData[] loadData()
        {
            // Define the directory path where the .mark files are located
            string directoryPath = "marks";

            // Get all .mark files in the specified directory
            string[] files = Directory.GetFiles(directoryPath, "*.mark");

            // Sort the files alphabetically by their file names
            var sortedFiles = files.OrderBy(f => Path.GetFileName(f)).ToArray();

            // Print the sorted files in a numbered list
            for (int i = 0; i < sortedFiles.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {Path.GetFileName(sortedFiles[i])}");
            }

            // Prompt the user to select a file by typing the corresponding number
            Console.WriteLine("Select a file by typing the number:");
            if (int.TryParse(Console.ReadLine(), out int selectedIndex) && selectedIndex > 0 && selectedIndex <= sortedFiles.Length)
            {
                // Get the selected file based on the user's input
                string selectedFile = sortedFiles[selectedIndex - 1];
                Console.WriteLine($"You selected: {Path.GetFileName(selectedFile)}");

                // Read the contents of the selected file
                string fileContents = File.ReadAllText(selectedFile);
                Console.WriteLine("File contents:");
                Console.WriteLine(fileContents);

                // Parse and save the data into an array
                return ParseAndSaveData(fileContents);
            }
            else
            {
                // Print an error message if the selection is invalid
                Console.WriteLine("Invalid selection.");
                return new StudentData[0];
            }
        }

        static StudentData[] ParseAndSaveData(string data)
        {
            // Define regex patterns to extract student and marks information
            string studentPattern = @"Student:\[ID:(\d+),LastName:(\w+),FirstName:(\w+)\],Marks:\[Challenges:\[(.*?)\],Exam:(\d+),Capstone:(\d+)\]";
            var studentMatches = Regex.Matches(data, studentPattern);

            // Create a list to store student data
            var studentDataList = new List<StudentData>();

            foreach (Match match in studentMatches)
            {
                if (match.Success)
                {
                    // Extract student information
                    string id = match.Groups[1].Value;
                    string lastName = match.Groups[2].Value;
                    string firstName = match.Groups[3].Value;

                    // Extract marks information
                    string challenges = match.Groups[4].Value;
                    string exam = match.Groups[5].Value;
                    string capstone = match.Groups[6].Value;

                    // Split challenges into groups of 5 and 3 and order them in descending order
                    int[] challengeArray = challenges.Split(',').Select(int.Parse).ToArray();
                    int[] challengesGroup1 = challengeArray.Take(5).OrderByDescending(x => x).ToArray();
                    int[] challengesGroup2 = challengeArray.Skip(5).Take(3).OrderByDescending(x => x).ToArray();

                    // Create a StudentData object and add it to the list
                    StudentData studentData = new StudentData
                    {
                        ID = id,
                        LastName = lastName,
                        FirstName = firstName,
                        Challenges = challengeArray,
                        ChallengesGroup1 = challengesGroup1,
                        ChallengesGroup2 = challengesGroup2,
                        Exam = int.Parse(exam),
                        Capstone = int.Parse(capstone)
                    };

                    studentDataList.Add(studentData);
                }
            }

            // Return the list as an array
            return studentDataList.ToArray();
        }

        static void PrintLoadedData(StudentData[] studentDataArray)
        {
            // Iterate over the array and print each student's data
            foreach (var studentData in studentDataArray)
            {
                Console.WriteLine($"Student ID: {studentData.ID}");
                Console.WriteLine($"Last Name: {studentData.LastName}");
                Console.WriteLine($"First Name: {studentData.FirstName}");
                Console.WriteLine($"Challenge Scores: {string.Join(", ", studentData.Challenges)}");
                Console.WriteLine($"First Challenges Group: {string.Join(", ", studentData.ChallengesGroup1)}");
                Console.WriteLine($"Second Challenges Group: {string.Join(", ", studentData.ChallengesGroup2)}");
                Console.WriteLine($"Exam: {studentData.Exam}");
                Console.WriteLine($"Capstone: {studentData.Capstone}");

                // Calculate and print the sum of the first 4 marks from ChallengesGroup1 and the first 2 marks from ChallengesGroup2
                int portfolio = studentData.ChallengesGroup1.Take(4).Sum() + studentData.ChallengesGroup2.Take(2).Sum();
                Console.WriteLine($"Sum of first 4 marks from ChallengesGroup1 and first 2 marks from ChallengesGroup2: {portfolio}");

                double doublePortfolio = portfolio;
                double doubleExam = studentData.Exam;
                double doubleCapstone = studentData.Capstone;

                double portfolioPercentage = 100 * doublePortfolio / 30;
                portfolioPercentage = Math.Round(portfolioPercentage, 2, MidpointRounding.AwayFromZero);

                double openBookExamPercentage = 100 * doubleExam / 20;
                openBookExamPercentage = Math.Round(openBookExamPercentage, 2, MidpointRounding.AwayFromZero);

                double capstoneProjectPercentage = 100 * doubleCapstone / 100;
                capstoneProjectPercentage = Math.Round(capstoneProjectPercentage, 2, MidpointRounding.AwayFromZero);

                double totalPercentage = (portfolioPercentage * 50 / 100) + (openBookExamPercentage * 25 / 100) + (capstoneProjectPercentage * 25 / 100);
                totalPercentage = Math.Round(totalPercentage, 2, MidpointRounding.AwayFromZero);

                Console.WriteLine($"Total Percentage: {totalPercentage}");
                Console.WriteLine();
            }
        }

        static void OutputDataToFile(StudentData[] studentDataArray, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var studentData in studentDataArray)
                {
                    // Calculate the total percentage
                    int portfolio = studentData.ChallengesGroup1.Take(4).Sum() + studentData.ChallengesGroup2.Take(2).Sum();
                    double doublePortfolio = portfolio;
                    double doubleExam = studentData.Exam;
                    double doubleCapstone = studentData.Capstone;

                    double portfolioPercentage = 100 * doublePortfolio / 30;
                    portfolioPercentage = Math.Round(portfolioPercentage, 2, MidpointRounding.AwayFromZero);

                    double openBookExamPercentage = 100 * doubleExam / 20;
                    openBookExamPercentage = Math.Round(openBookExamPercentage, 2, MidpointRounding.AwayFromZero);

                    double capstoneProjectPercentage = 100 * doubleCapstone / 100;
                    capstoneProjectPercentage = Math.Round(capstoneProjectPercentage, 2, MidpointRounding.AwayFromZero);

                    double totalPercentage = (portfolioPercentage * 50 / 100) + (openBookExamPercentage * 25 / 100) + (capstoneProjectPercentage * 25 / 100);
                    totalPercentage = Math.Round(totalPercentage, 2, MidpointRounding.AwayFromZero);

                    // Write the data to the file
                    writer.WriteLine($"{totalPercentage} - {studentData.FirstName} {studentData.LastName} ({studentData.ID})");
                }
            }
        }
    }

    class StudentData
    {
        // Properties to store student information
        public string ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int[] Challenges { get; set; }
        public int[] ChallengesGroup1 { get; set; }
        public int[] ChallengesGroup2 { get; set; }
        public int Exam { get; set; }
        public int Capstone { get; set; }
    }
}