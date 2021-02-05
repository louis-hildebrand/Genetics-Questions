// v2.1 (last updated 9/11/2019)
/* UPDATE LOG:
	v2.0
	- Added new question type!
		--> [2] Possible offspring genotypes and expected proportions
	- Fixed input validation issues

	v2.1
	- [1] Made "easy" questions (i.e. solution is 0 or 1) appear in only 10% of questions
	- Added some colours!
		--> User inputs: blue
		--> "Correct answer" messages: green 
		--> "Incorrect answer" messages: red 
		--> etc.
	
	v2.2
	- Fixed answer validation in Q2
*/

using System;

public class Program
{
	public static void Main()
	{
		string input = "";				// String to temporaily contain user inputs
		bool cont = true;				// Does the user want to see a new question type?
		int[] stats = {0, 0};			// Store session statistics ([0] #correct; [1] #attempts)
		int[] tempStats = new int[2];	// Temporary array to receive stats from individual question types
		string FORMAT = "{0, -13}{1}";	// Format for output
		bool errMsg = false;			// Whether the output is an error message or an original message
        
		// Intro screen
        Console.Clear();
		Console.Write("Welcome to the genetics study guide!" + 
            "\n\nA random question will be generated according to the parameters you select. You can then submit your answer and see the solution." + 
            "\nIf you notice a bug or believe the program's solution is incorrect or incomplete, please let me know via Messenger." +
			"\nI would also be happy to hear feedback or suggestions for other types of questions that can easily be randomly generated." +
			"\n\nPress any key to start.\n>> "
			);
		Console.ReadKey();
		Console.Clear();

		// Question type menu
		while (cont)
		{
			if (!errMsg)
			{
				Console.Clear();
				Console.Write(
					"Choose one of the question types below (type in the corresponding number)." +
					"\n\n[1] Probability of an offspring having [less than/at least/...] [n] [dominant/recessive] traits" +
					"\n[2] Possible offsprings and expected ratios ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("(**NOW AVAILABLE**)");
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("\n\n[x] See final score and quit" +
					"\n\n>> "
					);
				Console.ForegroundColor = ConsoleColor.Blue;
				input = Console.ReadLine();
				Console.ForegroundColor = ConsoleColor.White;
			}

			switch (input)
            {
				case "x":
					cont = false;
					break;
				case "1":	// Offpring phenotype probabilities
					errMsg = false;
					tempStats = Q1();
					stats[0] += tempStats[0]; stats[1] += tempStats[1];
					break;
				case "2":	// Possible offspring genotypes
					errMsg = false;
					tempStats = Q2();
					stats[0] += tempStats[0]; stats[1] += tempStats[1];
					break;
				default:
					errMsg = true;
					Console.Write("Error: invalid input. Please enter one of the listed question types.\n>> ");
					Console.ForegroundColor = ConsoleColor.Blue;
					input = Console.ReadLine();
					Console.ForegroundColor = ConsoleColor.White;
					break;
        	}
		}


		// Display session statistics
		Console.Clear();
        Console.WriteLine("\n" + FORMAT, "Attempted: ", stats[1]);
        Console.WriteLine(FORMAT, "Correct: ", stats[0]);
        Console.WriteLine("_________________\n");
        if (stats[1] == 0)
			Console.WriteLine("Final Score: -");
		else
			Console.WriteLine(FORMAT, "Final Score: ", Math.Round((double) stats[0]/stats[1]*100, 2, MidpointRounding.AwayFromZero));

		Console.WriteLine("\n\nThanks for using the genetics study guide! Best of luck on the test!");
	}
	

	////////////////////////////////////////////////////////////////
	//							QUESTIONS						  //
	////////////////////////////////////////////////////////////////


	public static int[] Q1()	// Probability of an offspring having [less than/at least/...] [n] [dominant/recessive] traits
	{
		const int MINTRAITS = 2, MAXTRAITS = 6;				// Bounds on the allowable number of traits
		string[] genotype = new string[2];					// Store's parents' genotypes
		double[,] prob;										// m*n array giving the probability that the offspring's mth trait is homozygous recessive (n=0), hetero (n=1), or homo dominant (n=2)
		string[,] solArray;									// See Google Sheet
		string[,] numGenotype;								// See Google Sheet
		int numTraits;										// Number of traits to consider for a given question
		int[] stats = {0, 0};								// Track user scores: [0] correct, [1] attempted
        string statFORMAT = "{0, -13}{1}", solFORMAT;		// Formats for outputs
		string temp;										// Temporary string
		string cont = "YES";								// Ask another question of the same sort?
		double answer, solution;							// User's answer and true solution
		int qTraits; string cond, type;						// Parameters of the question
		Random rand = new Random(); 						// Random number generator
		string r;											// String to store randomly generated numbers
		
		while (true) 
		{
            stats[1]++;
            Console.Clear();
            Console.WriteLine("[1] Problem #{0}\n", stats[1]);
            
			// User inputs number of traits
            Console.Write("How many traits would you like to consider? (Minumum of {0} and maximum of {1} traits)\n>> ", MINTRAITS, MAXTRAITS);
			Console.ForegroundColor = ConsoleColor.Blue;
            while(!int.TryParse(Console.ReadLine(), out numTraits) || numTraits < MINTRAITS || numTraits > MAXTRAITS)
            {
                Console.ForegroundColor = ConsoleColor.White;
				Console.Write("Error: invalid input. The number of traits must be a number between {0} and {1}.\n>> ", MINTRAITS, MAXTRAITS);
				Console.ForegroundColor = ConsoleColor.Blue;
            }
			Console.ForegroundColor = ConsoleColor.White;
            
			// Generate parents' genotypes
			numGenotype = new string[2, numTraits];
			for (int p=0; p<2; p++)
			{
				temp = "";
				
				for (int i=0; i<numTraits; i++)
				{
					r = rand.Next(3).ToString();
					temp += r;
					numGenotype[p, i] = r;
				}
				
				genotype[p] = toGenotype(temp);
			}

			// Find probability of homo recessive/hetero/homodominant genotype for each trait
			/*
			row index 0 = homozygous recessive for this trait, 1 = heterozygous, 2 = homozygous dominant
			col index = trait (~ row index in 'alleles')
			*/
			prob = new double[numTraits, 3];
			for (int i=0; i<numTraits; i++)
			{
				switch (numGenotype[0, i] + numGenotype[1, i])
				{
					case "00":
						prob[i, 0] = 1;
						prob[i, 1] = 0;
						prob[i, 2] = 0;
						break;
					case "01":
					case "10":
						prob[i, 0] = 0.5;
						prob[i, 1] = 0.5;
						prob[i, 2] = 0;
						break;
					case "02":
					case "20":
						prob[i, 0] = 0;
						prob[i, 1] = 1;
						prob[i, 2] = 0;
						break;
					case "11":
						prob[i, 0] = 0.25;
						prob[i, 1] = 0.5;
						prob[i, 2] = 0.25;
						break;
					case "12":
					case "21":
						prob[i, 0] = 0;
						prob[i, 1] = 0.5;
						prob[i, 2] = 0.5;
						break;
					case "22":
						prob[i, 0] = 0;
						prob[i, 1] = 0;
						prob[i, 2] = 1;
						break;
					default:
						Console.WriteLine("Error: invalid entry/entries in column {0} of numGenotype.", i);
						break;
				}
			}
			
			while (true)
			{
				// Generate question
				string[] typeArray = new string[2] {"recessive", "dominant"}, condArray = new string[7] {"exactly", "at least", "no fewer than", "more than", "at most", "no more than", "less than"};
				qTraits = rand.Next(numTraits);
				if (qTraits == 0)
					cond = "exactly";
				else 
					cond = condArray[rand.Next(7)];
				type = typeArray[rand.Next(2)];

				// Solve question
				// Create solution array with every possible genotype for the offspring, the product of probabilities (written out), and the final result
				solArray = new string[(int) Math.Pow(3, numTraits), 4];
				solution = 0;
				for (int i=0; i<Math.Pow(3, numTraits); i++)
				{
					// List all valid solutions (col 0)
					temp = DecimalToArbitrarySystem(i, 3, numTraits);
					solArray[i, 0] = toGenotype(temp);

					// Does the genotype satisfy the question conditions (col 4)?
					if (!isGoodGenotype(temp, qTraits, cond, type))
						solArray[i, 3] = "false";
					else
						solArray[i, 3] = "true";
					
					int t;
					// Write out product of probabilities (col 1) and find final answer (col 2)
					solArray[i, 2] = "1";
					for (int j=0; j<numTraits; j++)
					{
						t = (int) Char.GetNumericValue(temp[j]);
						solArray[i, 1] += (prob[j, t] + " * ");
						solArray[i, 2] = (double.Parse(solArray[i, 2]) * prob[j, t]).ToString();
					}
					solArray[i, 1] = solArray[i, 1].Substring(0, solArray[i, 1].Length-3);
					
					if (solArray[i, 3]=="true" && solArray[i, 2] != "0")
						solution += double.Parse(solArray[i, 2]);
				}
				solution = Math.Round(solution, 4, MidpointRounding.AwayFromZero);

				if (solution != 0 && solution != 1)
					break;
				if (rand.Next(101) <= 10)
					break;
			}
			
			// Output question
			Console.Clear();
			switch (qTraits)
			{
				case 1:
					temp = "trait";
					break;
				default:
					temp = "traits";
					break;
			}
            Console.WriteLine("[1] Problem #{0}\n", stats[1]);
            Console.Write("Consider the following couple: {0} x {1}." + 
            	"\nWhat is the probability that this couple's offspring will exhibit {2} {3} {4} {5}? Enter your answer with four significant figures after the decimal (if applicable).\n>> ",
				genotype[0], genotype[1], cond, qTraits, type, temp
				);
            Console.ForegroundColor = ConsoleColor.Blue;
			while(!double.TryParse(Console.ReadLine(), out answer) || answer > 1 || answer < 0)
            {
                Console.ForegroundColor = ConsoleColor.White;
				Console.Write("Error: invalid input. Your answer must be a number between 0 and 1.\n>> ");
				Console.ForegroundColor = ConsoleColor.Blue;
            }
			Console.ForegroundColor = ConsoleColor.White;

            if (answer == Math.Round(solution, 4, MidpointRounding.AwayFromZero))
            {
                Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("\nYour answer is correct!");
				Console.ForegroundColor = ConsoleColor.White;
                stats[0]++;
            }
            else
            {
				Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nThis answer is incorrect.");
				Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Correct value:\t{0}", solution);
            }

			// Display solution?
            Console.Write("\nWould you like to see the solution? Enter \"yes\" to see the solution or \"no\" to continue.\n>> ");
            Console.ForegroundColor = ConsoleColor.Blue;
			temp = Console.ReadLine().ToUpper().Trim();
			Console.ForegroundColor = ConsoleColor.White;
            while (!(temp == "YES" || temp == "Y" || temp == "NO" || temp == "N")) 
            {
                Console.Write("Error: invalid input. Enter \"yes\" to see the solution or \"no\" to continue.\n>> ");
				Console.ForegroundColor = ConsoleColor.Blue;
                temp = Console.ReadLine().ToUpper().Trim();
				Console.ForegroundColor = ConsoleColor.White;
            }
            switch (temp)
            {
                case "YES":
                case "Y":
					// Set solFORMAT (has to be done by hand because placeholders don't seem to accept variable widths)
					// Width of genotype placeholder = 2*numTraits, width of calculation placeholder = 7*numTraits - 3
					switch (numTraits)
					{
						case 2:
							solFORMAT = "{0, -8} | {1, -11} | {2, -10}";
							break;
						case 3:
							solFORMAT = "{0, -8} | {1, -18} | {2, -10}";
							break;
						case 4:
							solFORMAT = "{0, -8} | {1, -25} | {2, -10}";
							break;
						case 5:
							solFORMAT = "{0, -10} | {1, -32} | {2, -10}";
							break;
						default:
							solFORMAT = "{0, -12} | {1, -39} | {2, -10}";
							break;
					}

					Console.WriteLine("\n" + solFORMAT, "Genotype", "Calculation", "Result");
					for (int i=0; i<Math.Pow(3, numTraits); i++)
					{
						if (solArray[i, 3] == "true" && solArray[i, 2] != "0")
							Console.WriteLine(solFORMAT, solArray[i, 0], solArray[i, 1], solArray[i, 2]);
					}
					Console.WriteLine("\n" + solFORMAT, "", "Total", solution);

					Console.ForegroundColor = ConsoleColor.DarkYellow;
					Console.WriteLine("\n\n(Note: the solution displayed above is not necessarily the most effective approach to solving this particular problem." +
					"\nThe program simply lists all possible genotypes with {0} traits, filters out those that are impossible given the parents' genotypes" + 
					"\nor that do not meet the question's conditions, and calculates their probabilities. Before solving any problem, " + 
					"\nit is good to take a moment to think what approach is most effective in that particular situation.)\n", numTraits);
					Console.ForegroundColor = ConsoleColor.White;
                    break;
                default:
                    break;
            }
			
			// Try another problem?
            Console.Write("\nWould you like to try another problem of the same type? Enter \"yes\" to see another problem or \"no\" to see your score and return to the main menu.\n>> ");
            Console.ForegroundColor = ConsoleColor.Blue;
			cont = Console.ReadLine().ToUpper().Trim();
			Console.ForegroundColor = ConsoleColor.White;
            while(!(cont == "YES" || cont == "Y" || cont == "NO" || cont == "N"))
            {
                Console.Write("Error: invalid input. Enter \"yes\" to see another problem or \"no\" to see your score and return to the main menu.\n>> ");
                Console.ForegroundColor = ConsoleColor.Blue;
				cont = Console.ReadLine().ToUpper().Trim();
				Console.ForegroundColor = ConsoleColor.White;
            }
			
			if(cont == "NO" || cont == "N")
				break;
        }
        
		// If user doesn't want to try another problem, display statistics for this question type
		Console.Clear();
        Console.WriteLine( "\n" + statFORMAT, "Attempted: ", stats[1]);
        Console.WriteLine(statFORMAT, "Correct: ", stats[0]);
        Console.WriteLine("_________________\n");
        Console.WriteLine(statFORMAT + "%", "Score: ", Math.Round((double) stats[0]/stats[1]*100), 2, MidpointRounding.AwayFromZero);

		Console.Write("\nPress any key to continue.\n>> ");
		Console.ReadKey();

		return stats;
	}

	public static int[] Q2()	// Possible offsprings and expected ratios
	{
		int numTraits;														// Number of traits to consider for a given question
		const int MINTRAITS = 1, MAXTRAITS = 6;								// Bounds on the allowable number of traits
		string[] genotype = new string[2];									// Store's parents' genotypes
		double[,] prob;														// m*n array giving the probability that the offspring's mth trait is homozygous recessive (n=0), hetero (n=1), or homo dominant (n=2)
		string[,] bigSolArray, trimSolArray, ansArray, numGenotype;			// See Google Sheet
		int[] stats = {0, 0};												// Track user scores: [0] correct, [1] attempted
        string statFORMAT = "{0, -13}{1}", solFORMAT = "{0, -12} | {1}";	// Formats for outputs
		string temp;														// Temporary string
		double tempNum;
		string cont = "YES";												// Ask another question of the same sort?
		bool correct = true;												// Whether or not the user's answer is correct
		Random rand = new Random(); 										// Random number generator
		string r;															// String to store randomly generated numbers
		int msgLine, inLeft, inTop;
		
		while (true)
		{
			stats[1]++;
            Console.Clear();
            Console.WriteLine("[2] Problem #{0}\n", stats[1]);
			
			// User inputs number of traits
            Console.Write("How many traits would you like to consider? (Minumum of {0} and maximum of {1} traits)\n>> ", MINTRAITS, MAXTRAITS);
            Console.ForegroundColor = ConsoleColor.Blue;
			while(!int.TryParse(Console.ReadLine(), out numTraits) || numTraits < MINTRAITS || numTraits > MAXTRAITS)
            {
                Console.ForegroundColor = ConsoleColor.White;
				Console.Write("Error: invalid input. The number of traits must be a number between {0} and {1}.\n>> ", MINTRAITS, MAXTRAITS);
				Console.ForegroundColor = ConsoleColor.Blue;
            }
			Console.ForegroundColor = ConsoleColor.White;
            
			// Generate parents' genotypes
			numGenotype = new string[2, numTraits];
			for (int p=0; p<2; p++)
			{
				temp = "";
				
				for (int i=0; i<numTraits; i++)
				{
					r = rand.Next(3).ToString();
					temp += r;
					numGenotype[p, i] = r;
				}
				
				genotype[p] = toGenotype(temp);
			}

			// Find probability of homo recessive/hetero/homodominant genotype for each trait
			/*
			row index 0 = homozygous recessive for this trait, 1 = heterozygous, 2 = homozygous dominant
			col index = trait (~ row index in 'alleles')
			*/
			prob = new double[numTraits, 3];
			for (int i=0; i<numTraits; i++)
			{
				switch (numGenotype[0, i] + numGenotype[1, i])
				{
					case "00":
						prob[i, 0] = 1;
						prob[i, 1] = 0;
						prob[i, 2] = 0;
						break;
					case "01":
					case "10":
						prob[i, 0] = 0.5;
						prob[i, 1] = 0.5;
						prob[i, 2] = 0;
						break;
					case "02":
					case "20":
						prob[i, 0] = 0;
						prob[i, 1] = 1;
						prob[i, 2] = 0;
						break;
					case "11":
						prob[i, 0] = 0.25;
						prob[i, 1] = 0.5;
						prob[i, 2] = 0.25;
						break;
					case "12":
					case "21":
						prob[i, 0] = 0;
						prob[i, 1] = 0.5;
						prob[i, 2] = 0.5;
						break;
					case "22":
						prob[i, 0] = 0;
						prob[i, 1] = 0;
						prob[i, 2] = 1;
						break;
					default:
						Console.WriteLine("Error: invalid entry/entries in column {0} of numGenotype.", i);
						break;
				}
			}

			// Create solution array with list of all possible genotypes (col 0), probability (col 1), and whether or not the row appears in the user's answer (col 2)
			bigSolArray = new string[(int) Math.Pow(3, numTraits), 2];
			for (int i=0; i<Math.Pow(3, numTraits); i++)
			{
				// List all possible genotypes with number of traits = numTraits (col 0)
				temp = DecimalToArbitrarySystem(i, 3, numTraits);
				bigSolArray[i, 0] = toGenotype(temp);

				// Expected frequency of each genotype (col 1)
				int t;
				bigSolArray[i, 1] = "1";
				for (int j=0; j<numTraits; j++)
				{
					t = (int) Char.GetNumericValue(temp[j]);
					bigSolArray[i, 1] = (double.Parse(bigSolArray[i, 1]) * prob[j, t]).ToString();
				}
			}

			// Trim solution array
			int count = 0;		// Counts the number of genotypes in bigSolArray with non-zero expected frequencies
			for (int i=0; i<Math.Pow(3, numTraits); i++)
			{
				if (bigSolArray[i, 1] != "0")
					count++;
			}

			trimSolArray = new string[count, 2];
			count = 0;			// Track the row in trimSolArray as I go through bigSolArray row-by-row and assign non-zero entries to trimSolArray
			for (int i=0; i<Math.Pow(3, numTraits); i++)
			{
				if (bigSolArray[i, 1] != "0")
				{
					trimSolArray[count, 0] = bigSolArray[i, 0];
					trimSolArray[count, 1] = bigSolArray[i, 1];
					count++;
				}
			}
			
			// Output question and receive user's answer
			Console.Clear();
			Console.WriteLine("[2] Problem #{0}\n", stats[1]);
			Console.WriteLine("Consider the following couple: {0} x {1}" +
				"\nList all possible genotypes of their offspring, along with the expected proportions of each (with four " +
				"\nsignificant figures after the decimal, if applicable)." +
				"\n\nPress ENTER to move to the next cell in the table. To indicate that you have entered all your answers, type \"x_x\" into the first" +
				"\ncolumn of the table. Enter your answers carefully, as there is no going back once you have moved on to the next cell." +
				"\n\n", 
				genotype[0], genotype[1]);
			Console.WriteLine("EXAMPLE INPUT:" +
				"\n\n" + solFORMAT, "Genotype", "Expected Proportion" +
				"\n-------------|--------------------");
			Console.WriteLine(solFORMAT, "aabbcc", 0.7505);
			Console.WriteLine(solFORMAT, "AABBCC", 0.2495);
			Console.WriteLine(solFORMAT + "\n\n\n", "x_x", "");

			Console.WriteLine("\n\n" +solFORMAT, "Genotype", "Expected Proportion" + 
				"\n-------------|--------------------");
			
			int notBlank = 0;		// Counts the number of non-blank entries in ansArray
			msgLine = 19;
			ansArray = new string[(int) Math.Pow(3, numTraits), 2];
			for (int i=0; i<Math.Pow(3, numTraits); i++)
			{
				temp = Console.ReadLine();
				if (temp == "x_x")
					break;
				Console.SetCursorPosition(0, Console.CursorTop-1);
				Console.Write(solFORMAT, temp, "");
				ansArray[i, 0] = temp;
				notBlank++;

				inLeft = Console.CursorLeft; inTop = Console.CursorTop;
				while (!double.TryParse(Console.ReadLine(), out tempNum) || tempNum <= 0 || tempNum > 1)
				{
					Console.SetCursorPosition(0, msgLine);
					Console.WriteLine("                                                                                                                    ");
					Console.SetCursorPosition(0, msgLine);
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Error: invalid input. The expected frequency must be strictly greater than 0 and less than or equal to 1.");
					Console.ForegroundColor = ConsoleColor.White;
					Console.SetCursorPosition(inLeft, inTop);
					Console.WriteLine("                                                                                                                    ");
					Console.SetCursorPosition(inLeft, inTop);
				}
				Console.SetCursorPosition(0, msgLine);
				Console.WriteLine("                                                                                                                    ");
				Console.SetCursorPosition(0, msgLine);
				Console.SetCursorPosition(0, inTop+1);
				ansArray[i, 1] = tempNum.ToString();
			}

			for (int i=0; i<notBlank; i++)
			{
				Console.WriteLine("{0, -15} | {1}", ansArray[i, 0], ansArray[i, 1]);
			}

			// Validate user answer
			// Parse trimSolArray and check that every genotype appears in ansArray
			for (int i=0; i<(trimSolArray.Length/2); i++)
			{
				count = 0;		// Counts the number of matches for row i of trimSolArray in ansSolArray
				for (int j=0; j<notBlank; j++)
				{
					if (trimSolArray[i, 0] == ansArray[j, 0] && double.Parse(trimSolArray[i, 1]) == double.Parse(ansArray[j, 1]))	// WRONG CONDITION (!!!)
						count++;
				}
				if (count != 1)
					correct = false;
			}

			// Parse ansArray and check that every genotype appears in trimSolArray
			for (int i=0; i<notBlank; i++)
			{
				count = 0;		// Counts the number or matches for row i of trimSolArray in ansSolArray
				for (int j=0; j<(trimSolArray.Length/2); j++)
				{
					if (trimSolArray[j, 0] == ansArray[i, 0] && double.Parse(trimSolArray[j, 0]) == double.Parse(ansArray[i, 0]))
						count++;
				}
				if (count != 1)
					correct = false;
			}

			if (correct)
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("\nYour answer is correct!");
				Console.ForegroundColor = ConsoleColor.White;
				stats[0]++;
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("\nThis answer is incorrect.");
				Console.ForegroundColor = ConsoleColor.White;
			}
				

			// See solution?
			Console.Write("\nWould you like to see the solution? Enter \"yes\" to see the solution or \"no\" to continue.\n>> ");
			Console.ForegroundColor = ConsoleColor.Blue;
			temp = Console.ReadLine().ToUpper().Trim();
			Console.ForegroundColor = ConsoleColor.White;
			while (!(temp=="N" || temp=="NO" || temp=="Y" || temp=="YES"))
			{
				Console.Write("Error: invalid input. Enter \"yes\" to see the solution or \"no\" to continue.\n>> ");
				Console.ForegroundColor = ConsoleColor.Blue;
				temp = Console.ReadLine().ToUpper().Trim();
				Console.ForegroundColor = ConsoleColor.White;
			}
			if (temp=="Y" || temp=="YES")
			{
				Console.WriteLine("\n" + solFORMAT, "Genotype", "Expected Proportion" + 
					"\n-------------|--------------------");
				for (int i=0; i<(trimSolArray.Length/2); i++)
				{
					Console.WriteLine(solFORMAT, trimSolArray[i, 0], trimSolArray[i, 1]);
				}
			}

			// Try another problem?
            Console.Write("\nWould you like to try another problem of the same type? Enter \"yes\" to see another problem or \"no\" to see your score and return to the main menu.\n>> ");
            Console.ForegroundColor = ConsoleColor.Blue;
			cont = Console.ReadLine().ToUpper().Trim();
			Console.ForegroundColor = ConsoleColor.White;
            while(!(cont == "YES" || cont == "Y" || cont == "NO" || cont == "N"))
            {
                Console.Write("Error: invalid input. Enter \"yes\" to see another problem or \"no\" to see your score and return to the main menu.\n>> ");
                Console.ForegroundColor = ConsoleColor.Blue;
				cont = Console.ReadLine().ToUpper().Trim();
				Console.ForegroundColor = ConsoleColor.White;
            }
			
			if(cont == "NO" || cont == "N")
				break;
		}

		// If user doesn't want to try another problem, display statistics for this question type
		Console.Clear();
        Console.WriteLine("\n" + statFORMAT, "Attempted: ", stats[1]);
        Console.WriteLine(statFORMAT, "Correct: ", stats[0]);
        Console.WriteLine("_________________\n");
        Console.WriteLine(statFORMAT + "%", "Score: ", Math.Round((double) stats[0]/stats[1]*100), 2, MidpointRounding.AwayFromZero);

		Console.Write("\nPress any key to continue.\n>> ");
		Console.ReadKey();

		return stats;
	}

	////////////////////////////////////////////////////////////////////////
	//							UTILITY FUNCTIONS						  //
	////////////////////////////////////////////////////////////////////////

	/* 
	Converts a decimal number to the specified base 
	(Written by Pavel Vladov
	Accessed 3 Nov 2019 from StackOverflow: 
	https://stackoverflow.com/questions/923771/quickest-way-to-convert-a-base-10-number-to-any-base-in-net)
	*/
	public static string DecimalToArbitrarySystem(long decimalNumber, int radix, int numTraits)
	{
		const int BitsInLong = 64;
		const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		if (radix < 2 || radix > Digits.Length)
			throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());

		if (decimalNumber == 0)
			return new String('0', numTraits);

		int index = BitsInLong - 1;
		long currentNumber = Math.Abs(decimalNumber);
		char[] charArray = new char[BitsInLong];

		while (currentNumber != 0)
		{
			int remainder = (int)(currentNumber % radix);
			charArray[index--] = Digits[remainder];
			currentNumber = currentNumber / radix;
		}

		string result = new String(charArray, index + 1, BitsInLong - index - 1);
		if (decimalNumber < 0)
		{
			result = "-" + result;
		}

		// Added code to ensure that output always has length numTraits
		if (result.Length < numTraits)
		{
			result = new String('0', numTraits-result.Length) + result;
		}

		return result;
	}
	
	/*
	Receives a ternary number (as a string) and outputs a genotype (where 0 = homozygous recessive for a given trait, 1 = heterozygous, 2 = homozygous dominant)
	*/
	public static string toGenotype(string str)
	{
		string output = "";
		char[] alleles = {'p', 'q', 'r', 't', 'y', 'm'};
		
		for (int i=0; i<str.Length; i++)
		{
			switch (str[i])
			{
				case '0':
					output += (alleles[i].ToString() + alleles[i].ToString());
					break;
				case '1':
					output += ((alleles[i].ToString()).ToUpper() + alleles[i].ToString());
					break;
				case '2':
					output += ((alleles[i].ToString()).ToUpper() + (alleles[i].ToString()).ToUpper());
					break;
				default:
					Console.WriteLine("Error: invalid entry in genotype number string.");
					break;
			}
		}
		
		return output;
	}

	/*
	Checks if the genotype in temp meets the conditions (e.g. qTraits=3, cond="at least", type="recessive" means "Does this individual display at least 3 recessive traits?")
	*/
	public static bool isGoodGenotype(string temp, int qTraits, string cond, string type)
	{
		bool output = true;
		int good = 0;
		
		switch (type)
		{
			case "recessive":
				foreach (char c in temp) 
					if (c == '0') good++;
				break;
			case "dominant":
				foreach (char c in temp) 
					if (c == '1' || c == '2') good++;
				break;
			default:
				Console.WriteLine("Error: desired trait type not defined.");
				break;
		}
		
		switch (cond)
		{
			case "exactly":
				output = good == qTraits;
				break;
			case "at least":
			case "no fewer than":
				output = good >= qTraits;
				break;
			case "more than":
				output = good > qTraits;
				break;
			case "at most":
			case "no more than":
				output = good <= qTraits;
				break;
			case "less than":
				output = good < qTraits;
				break;
			default:
				Console.WriteLine("Error: condition not properly defined.");
				break;
		}
			
		return output;
	}
}