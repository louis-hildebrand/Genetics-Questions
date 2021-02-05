// NOTE TO ANYBODY INTERESTED IN PROGRAMMING: If you can think of new question types and you're willing to look through my spaghetti code, feel free to copy this program and add stuff to it!


// v4.0 (last updated 28/05/2020)
/* UPDATE LOG:
	v2.0
	- Added new question type!
		--> [2] Possible offspring genotypes and expected proportions
	- Fixed input validation issues

	v2.1
	- [1] Added filter to reduce the number of "boring" questions (i.e. solution is 0 or 1)
	- Added some colours!
		--> User inputs: blue
		--> "Correct answer" messages: green 
		--> "Incorrect answer" messages: red 
		--> etc.
	
	v3.0
	- Added new question type!
		--> [3] Hardy-Weinberg equilibrium

	v3.1
	- Added random question option in main menu
	- [3] To improve readability, added 1000s separators in population sizes and # of individuals

	v4.0
	- [1][3] Changed answer format from decimals to fractions
	- [1] Fixed filter, so boring questions (answer = 0 or 1) will now appear in only about 1/20 questions
	- Modified input validation so that the error message goes on the same line as the original message
	- Various formatting tweaks
*/


using System;

namespace Genetics_Questions
{
	public class Program
	{
		public static void Main()
		{
			string choice = "";				// String to temporaily contain user inputs
			bool cont = true;				// Does the user want to see a new question type?
			int[] stats = {0, 0};			// Store session statistics ([0] #correct; [1] #attempts)
			int[] tempStats = new int[2];	// Array to remporarily hold stats from individual question types
			string FORMAT = "{0, -13}{1}";	// Format for output
			bool errMsg = true;			// Whether the output is an error message or an original message
			Random rand = new Random();		// Random number generator
			int msgLine;					// Number of the line on which to print an error message if the user's input is invalid

			Console.Clear();
			Console.Write(
				"Welcome to the unofficial practice question generator for the genetics unit of General Biology I!" +
				"\nLast update: 2020/05/28" +
				"\n" +
				"\nIf you notice a bug, you can report it to me:" +
				"\n\tlouishildebrand@hotmail.ca" +
				"\nYou can also try using the older version, which was more extensively tested:" +
				"\n\trepl.it/@Lonelyboi2718/Genetics-Study-Guide-In-Progress" +
				"\n\n" + new String('_', 100) + "\n\n" +
				"\nChoose one of the question types below by typing in the corresponding number:" +
				"\n" +
				"\n[1] Probability of an offspring having [less than/at least/...] [n] [dominant/recessive] traits" +
				"\n[2] Possible offsprings and expected ratios" +
				"\n[3] Hardy-Weinberg equilibrium" +
				"\n\n[r] Get a random question" +
				"\n[x] See final score and quit" +
				"\n\n"
				);
			msgLine = Console.CursorTop;
			Console.Write("\n>> ");
			Console.ForegroundColor = ConsoleColor.Blue;
			choice = Console.ReadLine();
			Console.ForegroundColor = ConsoleColor.White;
		
			// Main menu
			while (cont)
			{
				if (!errMsg)
				{
					Console.Clear();
					Console.Write(
						"Choose one of the question types below by typing in the corresponding number:" +
						"\n\n[1] Probability of an offspring having [less than/at least/...] [n] [dominant/recessive] traits" +
						"\n[2] Possible offsprings and expected ratios" +
						"\n[3] Hardy-Weinberg equilibrium" +
						"\n\n[r] Get a random question" +
						"\n[x] See final score and quit" +
						"\n\n\n"
						);
					Console.Write(">> ");
					Console.ForegroundColor = ConsoleColor.Blue;
					choice = Console.ReadLine();
					Console.ForegroundColor = ConsoleColor.White;
				}

				switch (choice)
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
						Q2();
						break;
					case "3":
						errMsg = false;
						tempStats = Q3();
						stats[0] += tempStats[0]; stats[1] += tempStats[1];
						break;
					case "r":
						errMsg = true;		// Pretend there's an error so that the code loops back to the beginning of the switch statement
						choice = rand.Next(1, 4).ToString();
						break;
					default:
						errMsg = true;
						errorMsg(msgLine, "Error: invalid input. Please choose one of the listed question types.");
						choice = Console.ReadLine();
						Console.ForegroundColor = ConsoleColor.White;
						break;
        		}
			}


			// Display session statistics
			Console.Clear();
			Console.WriteLine(FORMAT, "Attempted: ", stats[1]);
			Console.WriteLine(FORMAT, "Correct: ", stats[0]);
			Console.WriteLine("_________________\n");
			if (stats[1] == 0)
			{
				Console.WriteLine(FORMAT, "Final score: ", "--");
			}
			else
			{
				Console.WriteLine(FORMAT + "%", "Final score: ", Math.Round((double) stats[0]/stats[1]*100, 2, MidpointRounding.AwayFromZero));
			}
		
			Console.WriteLine();
		
			Console.WriteLine("Thanks for using the genetics study guide! Best of luck on the test!");
			Console.WriteLine("\nPress any key to exit");
			Console.ReadLine();
		}
	

		////////////////////////////////////////////////////////////////
		//							QUESTIONS						  //
		////////////////////////////////////////////////////////////////


		public static int[] Q1()	// Probability of an offspring having [less than/at least/...] [n] [dominant/recessive] traits
		{
			const int MINTRAITS = 2, MAXTRAITS = 6;				// Bounds on the allowable number of traits
			string[,] numGenotype;								// Stores the parents' genotypes (one in each row) as numbers, where 0 = homozygous recessive, 1 = heterozygous, 2 = homozygous dominant. One column is allotted for each trait.
			string[] genotype = new string[2];					// Stores parents' genotypes
			Fraction[,] prob;									// m*n array giving the probability that the offspring's mth trait is homozygous recessive (n=0), hetero (n=1), or homo dominant (n=2)
			string[,] solArray;									// Column 0 lists all possible genotypes with the appropriate number of traits (e.g. ppQqRR). 
																// Column 1 shows the calculation to find the probability of that genotype given the specified parents. 
																// Column 2 is the final answer (i.e. the probability of that genotype given the specified parents).
																// Column 3 is a boolean showing whether the given genotype satisfies the question conditions.
			int numTraits;										// Number of traits to consider for a given question
			int[] stats = {0, 0};								// Track user scores: [0] correct, [1] attempted
			string statFORMAT = "{0, -13}{1}", solFORMAT;		// Formats for outputs
			string cont = "YES";								// Ask another question of the same sort?
			Fraction answer = new Fraction(0);					// User's answer
			Fraction solution = new Fraction(0);				// Correct answer to the question
			int qTraits = 0; 									// Number of traits the question asks about (e.g. "Probability of exactly *3* recessive?")
			string cond = "exactly", type = "recessive";		// Relational operator (e.g. "Probability of *exactly* 3 recessive?") and phenotype (e.g. "Probability of exactly 3 *recessive*?") in the question
			Random rand = new Random(); 						// Random number generator
			string temp;										// Temporary string
			string r;											// String to store randomly generated numbers
			int msgLine;										// Row number of the most recent message (so that an error message can be printed over the initial message, if necessary)
			const double BORING_NORMAL = 0.65;					// Proportion of questions that will be "boring" if none are filtered out (found to be approximately 32478/50000)
			const double BORING_WANTED = 0.05;					// Desired proportion of "boring" questions (i.e. answer = 0 or 1)
		
			while (true) 
			{
				stats[1]++;
				Console.Clear();
				Console.WriteLine("[1] OFFSPRING PHENOTYPE PROBABILITY (Question #{0})\n", stats[1]);
            
				// User inputs number of traits
				msgLine = Console.CursorTop;
				Console.Write("How many traits would you like to consider? (Minumum of {0} and maximum of {1} traits)\n>> ", MINTRAITS, MAXTRAITS);
				Console.ForegroundColor = ConsoleColor.Blue;
				while(!int.TryParse(Console.ReadLine(), out numTraits) || numTraits < MINTRAITS || numTraits > MAXTRAITS)
				{
					errorMsg(msgLine, "Error: invalid input. The number of traits must be a number between " + MINTRAITS + " and " + MAXTRAITS + ".");
				}
				Console.ForegroundColor = ConsoleColor.White;
            
				// Generate question and find the solution. If a question is boring (solution = 0 or 1), go back and create a new one (usually).
				while (true)
				{
					// Generate parents' genotypes
					numGenotype = new string[2, numTraits];
					for (int p = 0; p < 2; p++)
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
					prob = new Fraction[numTraits, 3];
					for (int i = 0; i < numTraits; i++)
					{
						switch (numGenotype[0, i] + numGenotype[1, i])
						{
							case "00":
								prob[i, 0] = new Fraction (1, 1);
								prob[i, 1] = new Fraction (0, 1);
								prob[i, 2] = new Fraction (0, 1);
								break;
							case "01":
							case "10":
								prob[i, 0] = new Fraction (1, 2);
								prob[i, 1] = new Fraction (1, 2);
								prob[i, 2] = new Fraction (0, 1);
								break;
							case "02":
							case "20":
								prob[i, 0] = new Fraction (0, 1);
								prob[i, 1] = new Fraction (1, 1);
								prob[i, 2] = new Fraction (0, 1);
								break;
							case "11":
								prob[i, 0] = new Fraction (1, 4);
								prob[i, 1] = new Fraction (1, 2);
								prob[i, 2] = new Fraction (1, 4);
								break;
							case "12":
							case "21":
								prob[i, 0] = new Fraction (0, 1);
								prob[i, 1] = new Fraction (1, 2);
								prob[i, 2] = new Fraction (1, 2);
								break;
							case "22":
								prob[i, 0] = new Fraction (0, 1);
								prob[i, 1] = new Fraction (0, 1);
								prob[i, 2] = new Fraction (1, 1);
								break;
							default:
								Console.WriteLine("Error: invalid entry/entries in column {0} of numGenotype.", i);
								break;
						}
					}
			
			
					// To solve question, create solution array with every possible genotype for the offspring, the product of probabilities (written out), and the final result
					solArray = new string[(int) Math.Pow(3, numTraits), 4];
					// Clear solArray
					for (int i = 0; i < solArray.Length/4; i++)
					{
						for (int j = 0; j < 4; j++)
						{
							solArray[i, j] = "";
						}
					}
				
					// Generate question
					string[] typeArray = new string[2] {"recessive", "dominant"}, condArray = new string[7] {"exactly", "at least", "no fewer than", "more than", "at most", "no more than", "less than"};
					qTraits = rand.Next(numTraits);
					if (qTraits == 0)
						cond = "exactly";
					else 
						cond = condArray[rand.Next(7)];
					type = typeArray[rand.Next(2)];

					// Solve question
					solution = new Fraction (0);
					Fraction tempFrac;
					for (int i = 0; i < Math.Pow(3, numTraits); i++)
					{
						// List all valid solutions (col 0)
						temp = DecimalToArbitrarySystem(i, 3, numTraits);
						solArray[i, 0] = toGenotype(temp);

						// Does the genotype satisfy the question conditions (col 3)?
						if (!isGoodGenotype(temp, qTraits, cond, type))
							solArray[i, 3] = "false";
						else
							solArray[i, 3] = "true";
					
						// Write out product of probabilities (col 1) and find final answer (col 2)
						int t;
						tempFrac = new Fraction (1);
						for (int j=0; j<numTraits; j++)
						{
							t = (int) Char.GetNumericValue(temp[j]);
							solArray[i, 1] += (prob[j, t].StringValueReduced + " * ");
							tempFrac *= prob[j, t];
						}
						solArray[i, 2] = tempFrac.StringValueReduced;
						solArray[i, 1] = solArray[i, 1].Substring(0, solArray[i, 1].Length-3);
					
						if (solArray[i, 3] == "true")
							solution = solution + tempFrac;
					}
				
					// If this question is considered "boring" (answer is 0 or 1), generate a number between 0 and 999 (inclusive). If the random number is less than k (where k is
					// chosen such that the proportion of boring questions outputted is equal to BORING_WANTED), the boring question is shown to the user. Otherwise, a new question is generated.
					if (solution == 0 || solution == 1)
					{
						if (rand.Next(1000) < 1000*BORING_WANTED*(1 - BORING_NORMAL)/((1 - BORING_WANTED)*BORING_NORMAL))
							break;
					}
					else
					{
						break;
					}
				}
			
				// Output question
				Console.Clear();
				Console.WriteLine("[1] OFFSPRING PHENOTYPE PROBABILITY (Question #{0})\n", stats[1]);

				switch (qTraits)
				{
					case 1:
						temp = "trait";
						break;
					default:
						temp = "traits";
						break;
				}
				Console.WriteLine("Consider the following couple: {0} x {1}." + 
            		"\nWhat is the probability that this couple's offspring will exhibit {2} {3} {4} {5}?\n",
					genotype[0], genotype[1], cond, qTraits, type, temp
					);
				msgLine = Console.CursorTop;
				Console.Write("Enter your answer as a fraction (e.g. >> 3/8) or as an integer (e.g. >> 1).\n>> ");
            
				// Receive and validate user answer
				Console.ForegroundColor = ConsoleColor.Blue;
				temp = Console.ReadLine();
				while(!isValidFractionalProb(temp))
				{
					errorMsg(msgLine, "Error: your answer must be a fraction between 0 and 1. Enter a valid fraction (e.g. >> 3/8) or integer (e.g. >> 1).");
					temp = Console.ReadLine();
				}
				Console.ForegroundColor = ConsoleColor.White;
				answer = Fraction.Parse(temp);

				if (answer == solution)
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
					Console.WriteLine("Correct value:\t{0}", solution.StringValueReduced);
				}

				// Display solution?
				Console.WriteLine();
				msgLine = Console.CursorTop;
				Console.Write("Would you like to see the solution? Enter \"yes\" to see the solution or \"no\" to continue.\n>> ");
				Console.ForegroundColor = ConsoleColor.Blue;
				temp = Console.ReadLine().ToUpper().Trim();
				while (!(temp == "YES" || temp == "Y" || temp == "NO" || temp == "N")) 
				{
					errorMsg(msgLine, "Error: invalid input. Enter \"yes\" to see the solution or \"no\" to continue.");
					temp = Console.ReadLine().ToUpper().Trim();
				}
				Console.ForegroundColor = ConsoleColor.White;
				switch (temp)
				{
					case "YES":
					case "Y":
						// Set solFORMAT (has to be done by hand because placeholders don't seem to accept variable widths)
						// Width of genotype placeholder = 2*numTraits, width of calculation placeholder = 7*numTraits - 3
						switch (numTraits)
						{
							case 2:
								solFORMAT = "{0, -8} | {1, -15} | {2, -10}";
								break;
							case 3:
								solFORMAT = "{0, -8} | {1, -23} | {2, -10}";
								break;
							case 4:
								solFORMAT = "{0, -8} | {1, -31} | {2, -10}";
								break;
							case 5:
								solFORMAT = "{0, -10} | {1, -39} | {2, -10}";
								break;
							default:
								solFORMAT = "{0, -12} | {1, -47} | {2, -10}";
								break;
						}

						Console.WriteLine();
						Console.WriteLine(new String('_', 100));
						Console.WriteLine();
						Console.WriteLine(solFORMAT, "Genotype", "Calculation", "Result");
						for (int i=0; i<Math.Pow(3, numTraits); i++)
						{
							if (solArray[i, 3] == "true" && solArray[i, 2] != "0")
								Console.WriteLine(solFORMAT, solArray[i, 0], solArray[i, 1], solArray[i, 2]);
						}
						Console.WriteLine("\n" + solFORMAT, "", "Total", solution.StringValueReduced);
						Console.WriteLine(new String('_', 100));

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
				Console.WriteLine();
				msgLine = Console.CursorTop;	
				Console.Write("Would you like to try another problem of the same type? Enter \"yes\" to see another problem or \"no\" to see your score and return to the main menu.\n>> ");
				Console.ForegroundColor = ConsoleColor.Blue;
				cont = Console.ReadLine().ToUpper().Trim();
				while(!(cont == "YES" || cont == "Y" || cont == "NO" || cont == "N"))
				{
					errorMsg(msgLine, "Error: invalid input. Enter \"yes\" to see another problem or \"no\" to see your score and return to the main menu.");
					cont = Console.ReadLine().ToUpper().Trim();
				}
				Console.ForegroundColor = ConsoleColor.White;
			
				if(cont == "NO" || cont == "N")
					break;
			}
        
			// If user doesn't want to try another problem, display statistics for this question type
			Console.Clear();
			Console.WriteLine("[1] OFFSPRING PHENOTYPE PROBABILITY\n");

			Console.WriteLine(statFORMAT, "Attempted: ", stats[1]);
			Console.WriteLine(statFORMAT, "Correct: ", stats[0]);
			Console.WriteLine("_________________");
			Console.WriteLine(statFORMAT + "%", "Score: ", Math.Round((double) stats[0]/stats[1]*100), 2, MidpointRounding.AwayFromZero);

			Console.Write("\nPress ENTER to continue.");
			ConsoleKeyInfo cki;
			while (true)
			{
				cki = Console.ReadKey(true);
				if (cki.Key == ConsoleKey.Enter)
					break;
			}

			return stats;
		}

		public static void Q2()	// Possible offsprings and expected ratios
		{
			int numTraits;										// Number of traits to consider for a given question
			const int MINTRAITS = 1, MAXTRAITS = 6;				// Bounds on the allowable number of traits
			string[] genotype = new string[2];					// Store's parents' genotypes
			Fraction[,] prob;									// m*n array giving the probability that the offspring's mth trait is homozygous recessive (n=0), hetero (n=1), or homo dominant (n=2)
			string[,] bigSolArray, trimSolArray, numGenotype;	// See the corresponding comment in Q1(). trimSolArray has the same layout as ansArray from Q1() and bigSolArray, but all the genotypes that have a frequency of zero are removed.
			string solFORMAT;									// Format for displaying the solution
			string temp;										// Temporary string
			string cont = "YES";								// Ask another question of the same sort?
			int numTries = 0;									// Number of questions the user has attempted
			Random rand = new Random(); 						// Random number generator
			string r;											// String to store randomly generated numbers
			int msgLine;										// Row number of the most recent message (so that an error message can be printed over the initial message, if necessary)
		
			while (true)
			{
				numTries++;
				Console.Clear();
				Console.WriteLine("[2] EXPECTED OFFSPRING GENOTYPE RATIOS (Question #{0})\n", numTries);
			
				// User inputs number of traits
				msgLine = Console.CursorTop;
				Console.Write("How many traits would you like to consider? (Minumum of {0} and maximum of {1} traits)\n>> ", MINTRAITS, MAXTRAITS);
				Console.ForegroundColor = ConsoleColor.Blue;
				while(!int.TryParse(Console.ReadLine(), out numTraits) || numTraits < MINTRAITS || numTraits > MAXTRAITS)
				{
					errorMsg(msgLine, "Error: invalid input. The number of traits must be a number between " + MINTRAITS + " and " + MAXTRAITS + ".");
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
				prob = new Fraction[numTraits, 3];
				for (int i = 0; i < numTraits; i++)
				{
					switch (numGenotype[0, i] + numGenotype[1, i])
					{
						case "00":
							prob[i, 0] = new Fraction(1);
							prob[i, 1] = new Fraction(0);
							prob[i, 2] = new Fraction(0);
							break;
						case "01":
						case "10":
							prob[i, 0] = new Fraction(1, 2);
							prob[i, 1] = new Fraction(1, 2);
							prob[i, 2] = new Fraction(0);
							break;
						case "02":
						case "20":
							prob[i, 0] = new Fraction(0);
							prob[i, 1] = new Fraction(1);
							prob[i, 2] = new Fraction(0);
							break;
						case "11":
							prob[i, 0] = new Fraction(1, 4);
							prob[i, 1] = new Fraction(1, 2);
							prob[i, 2] = new Fraction(1, 4);
							break;
						case "12":
						case "21":
							prob[i, 0] = new Fraction(0);
							prob[i, 1] = new Fraction(1, 2);
							prob[i, 2] = new Fraction(1, 2);
							break;
						case "22":
							prob[i, 0] = new Fraction(0);
							prob[i, 1] = new Fraction(0);
							prob[i, 2] = new Fraction(1);
							break;
						default:
							Console.WriteLine("Error: invalid entry/entries in column {0} of numGenotype.", i);
							break;
					}
				}

				// Create solution array with list of all possible genotypes (col 0), calculation (col 1), and probability (col 2)
				bigSolArray = new string[(int) Math.Pow(3, numTraits), 3];
				Fraction tempFrac;
				for (int i = 0; i < Math.Pow(3, numTraits); i++)
				{
					// List all possible genotypes with number of traits = numTraits (col 0)
					temp = DecimalToArbitrarySystem(i, 3, numTraits);
					bigSolArray[i, 0] = toGenotype(temp);

					// Expected frequency of each genotype: calculation (col 1) and final answer (col 2)
					int t;
					tempFrac = new Fraction (1);
					for (int j = 0; j < numTraits; j++)
					{
						t = (int) Char.GetNumericValue(temp[j]);
						bigSolArray[i, 1] += (prob[j, t].StringValueReduced + " * ");
						tempFrac *= prob[j, t];
					}
					bigSolArray[i, 2] = tempFrac.StringValueReduced;
					bigSolArray[i, 1] = bigSolArray[i, 1].Substring(0, bigSolArray[i, 1].Length-3);
				}

				// Trim solution array
				int count = 0;		// Counts the number of genotypes in bigSolArray with non-zero expected frequencies
				for (int i = 0; i < Math.Pow(3, numTraits); i++)
				{
					if (bigSolArray[i, 2] != "0")
						count++;
				}

				trimSolArray = new string[count, 3];
				count = 0;			// Track the row in trimSolArray as I go through bigSolArray row-by-row and assign non-zero entries to trimSolArray
				for (int i = 0; i < Math.Pow(3, numTraits); i++)
				{
					if (bigSolArray[i, 2] != "0")
					{
						trimSolArray[count, 0] = bigSolArray[i, 0];
						trimSolArray[count, 1] = bigSolArray[i, 1];
						trimSolArray[count, 2] = bigSolArray[i, 2];
						count++;
					}
				}
			
				// Output question
				Console.Clear();
				Console.WriteLine("[2] EXPECTED OFFSPRING GENOTYPE RATIOS (Question #{0})\n", numTries);

				Console.WriteLine("Consider the following couple: {0} x {1}" +
					"\n\nList all possible genotypes of their offspring, along with the expected proportions of each." +
					"\nWhen you are ready, press ENTER to reveal the answer." +
					"\n\n", 
					genotype[0], genotype[1]);
			
				// Set solFORMAT (has to be done by hand because placeholders don't seem to accept variable widths)
				// Width of genotype placeholder = 2*numTraits, width of calculation placeholder = 7*numTraits - 3
				switch (numTraits)
				{
					case 2:
						solFORMAT = "{0, -8} | {1, -15} | {2}";
						break;
					case 3:
						solFORMAT = "{0, -8} | {1, -23} | {2}";
						break;
					case 4:
						solFORMAT = "{0, -8} | {1, -31} | {2}";
						break;
					case 5:
						solFORMAT = "{0, -10} | {1, -39} | {2}";
						break;
					default:
						solFORMAT = "{0, -12} | {1, -47} | {2}";
						break;
				}
			
				// When user presses ENTER, show solution
				ConsoleKeyInfo cki;
				while (true)
				{
					cki = Console.ReadKey(true);
					if (cki.Key == ConsoleKey.Enter)
						break;
				}
				Console.WriteLine(new String('_', 126));
				Console.WriteLine("SOLUTION:");
				Console.WriteLine("\n" + solFORMAT, "Genotype", "Calculation", "Expected Proportion");
				for (int i = 0; i < (trimSolArray.Length/3); i++)
				{
					Console.WriteLine(solFORMAT, trimSolArray[i, 0], trimSolArray[i, 1], trimSolArray[i, 2]);
				}
				Console.WriteLine(new String('_', 126));

				// Try another problem?
				Console.WriteLine("\n");
				msgLine = Console.CursorTop;
				Console.Write("Would you like to try another problem of the same type? Enter \"yes\" to see another problem or \"no\" to return to the main menu.\n>> ");
				Console.ForegroundColor = ConsoleColor.Blue;
				cont = Console.ReadLine().ToUpper().Trim();
				while(!(cont == "YES" || cont == "Y" || cont == "NO" || cont == "N"))
				{
					errorMsg(msgLine, "Error: invalid input. Enter \"yes\" to see another problem or \"no\" to see your score and return to the main menu.");
					cont = Console.ReadLine().ToUpper().Trim();
				}
				Console.ForegroundColor = ConsoleColor.White;
			
				if(cont == "NO" || cont == "N")
					break;
			}
		}

		public static int[] Q3 ()	// Hardy-Weinberg equilibrium
		{
			const double MIN_PROPORTION = 0.2, MAX_PROPORTION = 0.8;
		
			int[] stats = {0, 0};
			double givenProportion, solution = 0, answer = 0;					// The proportion of affected individuals given by the question, the solution, and the user's answer
			double p, q;														// The frequency of alleles (p = dominant, q = recessive) and the proportion of each genotype in the population
			string givenPhenotype, questionGenotype;							// The phenotype given by the question and the genotype asked for by the question (0 to 3 if questionType[2] == 0, 4 to 5 if questionType[2] == 1)
			int popSize, affectedNum;											// Population size and number of affected individuals in the population
			string question;													// The question shown to the user
			string cont = "YES";												// Does the user want to see another question of the same type?
			string statFORMAT = "{0, -13}{1}";									// Formats for outputting statistics for this question type
			int msgLine;														// Row number of the most recent message (so that an error message can be printed over the initial message, if necessary)
			Random rand = new Random();											// Random number generator
			int[] questionType = new int[3];									// Determines the way the question is formulated
				// questionType[0]: 0 = user is given the proportion of affected individuals directly OR 1 = user is given number of individuals
				// questionType[1]: 0 = user is asked for proportion of population with desired genotype OR 1 = user is asked for number of individuals with genotype
				// questionType[2]: 0 = user is asked for overall genotype (e.g. "How many are heterozygous?") OR 1 = user is asked for individual alleles (e.g. "How many have a copy of dominant allele?")
			string[] phenotypes = {"dominant", "recessive"};					// Possible phenotypes from which to choose
			string[] genotypes = {"homozygous", "homozygous dominant", 			// Possible genotypes from which to choose questionGenotype
				"heterozygous", "homozygous recessive",
				"dominant", "recessive"};

			while (true)
			{
				stats[1]++;
				Console.Clear();
				Console.WriteLine("[3] HARDY-WEINBERG EQUILIBRIUM (Question #{0})\n", stats[1]);

				// Generate question parameters
				for (int i=0; i<3; i++)
				{
					questionType[i] = rand.Next(0, 2);
				}
				popSize = rand.Next(1000001);
				affectedNum = rand.Next((int) Math.Floor(MIN_PROPORTION*popSize), (int) Math.Floor(MAX_PROPORTION*popSize));
				givenProportion = (double) affectedNum / popSize;
				if (questionType[0] == 0)
					givenProportion = Math.Round(givenProportion, 4, MidpointRounding.AwayFromZero);
				givenPhenotype = phenotypes[rand.Next(0, 2)];
			
				if (questionType[2] == 0) {
					questionGenotype = genotypes[rand.Next(0, 4)];
				} else {
					questionGenotype = genotypes[rand.Next(4, 6)];
				}

				// Solve question (!!!)
				if (givenPhenotype == "recessive") {
					q = Math.Sqrt(givenProportion);
				} else {
					q = Math.Sqrt(1 - givenProportion);
				}
				p = 1 - q;

				if (questionGenotype == "homozygous") {
					solution = Math.Pow(p, 2) + Math.Pow(q, 2);
				} else if (questionGenotype == "homozygous dominant") {
					solution = Math.Pow(p, 2);
				} else if (questionGenotype == "heterozygous") {
					solution = 2*p*q;
				} else if (questionGenotype == "homozygous recessive") {
					solution = Math.Pow(q, 2);
				} else if (questionGenotype == "dominant") {
					solution = Math.Pow(p, 2) + 2*p*q;
				} else if (questionGenotype == "recessive") {
					solution = Math.Pow(q, 2) + 2*p*q;
				}

				if (questionType[1] == 1) {
					solution *= popSize;
					solution = Math.Round(solution, 0, MidpointRounding.AwayFromZero);
				} else {
					solution = Math.Round(solution, 4, MidpointRounding.AwayFromZero);
				}
			
				// Generate and output question statement
				Console.Clear();
				Console.WriteLine("[3] HARDY-WEINBERG EQUILIBRIUM (Question #{0})\n", stats[1]);

				question = "";

				if (questionType[0] == 0) {
					question += "Suppose that approximately " + givenProportion*100 + "% of indviduals in a population display a certain " + givenPhenotype + " trait. ";
				} else {
					question += "Suppose that, in a population of " + popSize.ToString("N0") + ", " + Math.Floor(givenProportion*popSize).ToString("N0") + " individuals display a certain " + givenPhenotype + " trait. ";
				}
				if (questionType[1] == 0) {
					question += "\nAssuming Hardy-Weinberg conditions, what proportion of the individuals in this population ";
				} else if (questionType[1] == 1 && questionType[0] == 0) {
					question += "The population size is " + popSize.ToString("N0") + ". \nAssuming Hardy-Weinberg conditions, how many individuals in this population ";
				} else {
					question += "\nAssuming Hardy-Weinberg conditions, how many individuals in this population ";
				}
				if (questionType[2] == 0) {
					question += "\nare " + questionGenotype + " for the trait in question?";
				} else {
					question += "\nhave at least one copy of the " + questionGenotype + " allele for the trait in question?";
				}
				if (questionType[1] == 0) {
					question += "\n\n(NOTE): Enter your answer with four significant figures after the decimal (if applicable). \n        Be sure to carry all digits between steps, as there is no tolerance for rounding errors.";
				} else {
					question += "\n\n(NOTE): Your answer should be rounded to the nearest integer, whether that means rounding up or down.";
				}
			
				Console.Write(question);
				Console.WriteLine("\n");
				msgLine = Console.CursorTop;
				Console.Write("\n>> ");
			
				// Receive and validate user answer
				bool error = true;
				while (error)	// Check for wack answers (not a number, negative, proportion greater than 1, # of individuals not an integer, etc.)
				{
					error = !double.TryParse(Console.ReadLine(), out answer);
					if (error) {
						errorMsg(msgLine, "Error: your answer must be a number.");
						continue;
					}
				
					if (answer < 0) {
						error = true;
						errorMsg(msgLine, "Error: your answer must be positive.");
						continue;
					}
				
					if (questionType[1] == 0 && answer > 1) {
						error = true;
						errorMsg(msgLine, "Error: your answer must be a number between 0 and 1.");
						continue;
					}

					if (questionType[1] == 1 && !int.TryParse(answer.ToString(), out int garbage)) {
						error = true;
						errorMsg(msgLine, "Error: your answer must be an integer.");
						continue;
					}
				}

				if (answer == solution) {
					Console.ForegroundColor = ConsoleColor.Green;
					stats[0]++;
					Console.WriteLine("\nYour answer is correct!");
					Console.ForegroundColor = ConsoleColor.White;
				} else {
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("\nThis answer is incorrect.");
					Console.ForegroundColor = ConsoleColor.White;
					if (questionType[1] == 0)
						Console.WriteLine("Correct answer: {0}", solution);
					else
						Console.WriteLine("Correct answer: {0}", solution.ToString("N0"));
				}

				// Try another problem?
				Console.WriteLine();
				msgLine = Console.CursorTop;
				Console.Write("Would you like to try another problem of the same type? Enter \"yes\" to see another problem or \"no\" to see your score and return to the main menu.\n>> ");
				Console.ForegroundColor = ConsoleColor.Blue;
				cont = Console.ReadLine().ToUpper().Trim();
				Console.ForegroundColor = ConsoleColor.White;
				while(!(cont == "YES" || cont == "Y" || cont == "NO" || cont == "N"))
				{
					errorMsg(msgLine, "Error: invalid input. Enter \"yes\" to see another problem or \"no\" to see your score and return to the main menu.");
					cont = Console.ReadLine().ToUpper().Trim();
				}
				Console.ForegroundColor = ConsoleColor.White;
			
				if(cont == "NO" || cont == "N")
					break;
			}

			// If user doesn't want to try another problem, display statistics for this question type
			Console.Clear();
			Console.WriteLine("[3] HARDY-WEINBERG EQUILIBRIUM\n", stats[1]);

			Console.WriteLine(statFORMAT, "Attempted: ", stats[1]);
			Console.WriteLine(statFORMAT, "Correct: ", stats[0]);
			Console.WriteLine("_________________");
			Console.WriteLine(statFORMAT + "%", "Score: ", Math.Round((double) stats[0]/stats[1]*100), 2, MidpointRounding.AwayFromZero);

			Console.Write("\nPress ENTER to continue.");
			ConsoleKeyInfo cki;
			while (true)
			{
				cki = Console.ReadKey(true);
				if (cki.Key == ConsoleKey.Enter)
					break;
			}

			return stats;
		}

		////////////////////////////////////////////////////////////////////////
		//							UTILITY FUNCTIONS						  //
		////////////////////////////////////////////////////////////////////////

		// Converts a decimal number to the specified base 
		// (Originally written by Pavel Vladov, modified slightly so that the output always has numTraits digits.
		// Accessed 3 Nov 2019 from StackOverflow: 
		// https://stackoverflow.com/questions/923771/quickest-way-to-convert-a-base-10-number-to-any-base-in-net)
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
	
		// Receives a ternary number (as a string) and outputs a genotype (where 0 = homozygous recessive for a given trait, 1 = heterozygous, 2 = homozygous dominant)
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

		// Checks if the genotype in temp meets the conditions (e.g. qTraits=3, cond="at least", type="recessive" means "Does this individual display at least 3 recessive traits?")
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

		// Checks that a string is a valid fraction AND that its value is between 0 and 1 (inclusive)
		public static bool isValidFractionalProb (string str)
		{
			if (!Fraction.TryParse(str))
			{
				return false;
			}
		
			Fraction frac = Fraction.Parse(str);
		
			if (frac < 0 || frac > 1)
			{
				return false;
			}

			return true;
		}

		// Print an error message in red on line msgLine
		public static void errorMsg (int msgLine, string msg)
		{
			Console.SetCursorPosition(0, msgLine);
			Console.Write(new String(' ', 3*Console.WindowWidth));
			Console.SetCursorPosition(0, msgLine);
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(msg);
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write(">> ");
			Console.ForegroundColor = ConsoleColor.Blue;
		}
	}
}