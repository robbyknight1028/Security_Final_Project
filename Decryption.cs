using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SecurityCourseProject
{
    public class Decryption
    {
        public string plainText;
        public string decCaesar;
        public string keyWordInput;
        public string testString;        

        // For Vigenere
        public char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray();
        List<int> encryptedInts = new List<int>();
        List<int> decryptedInts = new List<int>();
        List<int> keyInts = new List<int>();

        public Decryption()
        {
            this.plainText = "No Input.";
        }
        public Decryption(string plainText, string keyWordInput)
        {
            this.plainText = plainText;
            this.keyWordInput = keyWordInput;            
            //System.Diagnostics.Debug.WriteLine("Decryption Created");

        }



        public string PrintPlainText()
        {
            return plainText;
        }




        //----------Caesar Cipher---------------------------------------
        // To check all 25 shifts we'll have to loop and call this. -SR 3/18/21
        // Input a single char and it will shift by the integer given
        // works for positives or negatives
        public char BaseCaesar(char InText, int Shift)
        {
            char pivot = 'a';
            return (char)((((InText + Shift) - pivot) % 26) + pivot);
        }
        public string[] DecryptCaesar(string plainText)
        {          

            string[] bulkDecryptions = new string[25];

            // Clean up plainText for usability
            string trim_plainText = plainText.Trim(); // remove all leading and trailing white-space characters
            string remove_whitespace = "\\s+"; // find all mid string white-space characters with Regex
            string replace = "";
            string InText = Regex.Replace(trim_plainText, remove_whitespace, replace);            
            InText = InText.ToLower(); // change all to lowercase
            char[] encTextArr = InText.ToCharArray();
            char[] decTextArr = new char[encTextArr.Length];            

            // Need to check all 25 shifts
            // Decrypt uses negative values
            for (int i = 0; i < 25; i++)
            {
                int shift = i + 1;
                for (int j = 0; j < encTextArr.Length; j++)
                {
                    decTextArr[j] = BaseCaesar(encTextArr[j],shift);
                }
                string potDecrypt = new string(decTextArr);
                bulkDecryptions[i] = potDecrypt;
            }

            string[] bulkDecryptionsShortened = new string[] { };
            bulkDecryptionsShortened = WeightDigramTrigram(bulkDecryptions);
            return bulkDecryptionsShortened;
        }
        public string[] PrintCaesarCipher()
        {
            // testing the dictionary
            //if(englishDictionary.ContainsValue("umpirages"))
            //{
            //    string x = "Present in dictionary";
            //    return x;
            //}
            //else
            //{
            //    string y = "Not present in dictionary";

            //    return y;
            //}
            
            return DecryptCaesar(plainText);            
        }
        //----------END Caesar Cipher-------------------------------------


        
        
        //----------Columnar Cipher---------------------------------------
        //Potentially we should have this variable at the top and just set it once when creating the decipher object.
        public string providedText;
        //Main function
        //public List<string> DecryptColumnar(string plainText) -SR plaintext is available, updated 4/20/21
        public string[] DecryptColumnar()
        {
            //The point of these lines is to filter out 'spaces' in the encrypted text.
            //Not sure if this has been implemented already--- if not we should just add this before sending the "plainText"
            //------------------------------------------------------------
            providedText = plainText;
		    string providedText2 = "";
		    for(int j = 0; j < providedText.Count(); j++)
		    {
			    if (providedText[j] != ' ')
			    {
				    providedText2 += providedText[j];
			    }
		    }
		    providedText = providedText2;
		    //------------------------------------------------------------		
		
		    List<string> outputs = DecipherColumnar();
            string [] outputs2 = outputs.ToArray();
            string[] columnarCipherShortened = new string[] { };            
            columnarCipherShortened = WeightDigramTrigram(outputs2);
            return columnarCipherShortened;
	    }	    
	    //secondary function
	    public List<string> DecipherColumnar()
	    {
		    List<string> outputs = new List<string>();
		
		    //Goes through and and calls the actual cipher for each possible column size
		    for (int i = 1; i < providedText.Count(); i++)
		    {
			    outputs.Add(adjust(i));
		    }
            return outputs;

	    }
	    //main cipher
	    public string adjust(int columnSize)
	    {
		    string adjusted = "";
		
		    // if the column size is X, then it would follow that you could only fit N entries into (N/X) columns.
		    //------------------------------------------------------------
		    int letterCount = providedText.Count();
		    float columnCount1 = (float)letterCount / (float)columnSize;
		    int columnCount = (int)(Math.Floor(columnCount1));
		    //------------------------------------------------------------
		
		    string providedText2 = providedText;
		    // We need to fill in the empty spots at the end with some filler...
		    //------------------------------------------------------------
		    int neededSize = columnCount * columnSize;
		    int difference = neededSize - providedText.Count();
		
		    for (int i = difference; i > 0; i--)
		    {
			    providedText2+= "q";
		    }
		    //------------------------------------------------------------
		    int currenPos = 0;
		    string[] rows = new string[columnSize];
		
		    //Populate each row by adding characters from the plain text... the size of each row should be the column count
		    for (int i = 0; i < columnSize; i++)
		    {
			    for (int j = 0; j < columnCount; j++)
			    {
				    //while in previous lines I already made sure that the text was divisible by the column count, this is just a safety check just in case
				    //------------------------------------------------------------
				    if (providedText2.Count() > currenPos)
				    {
					    rows[i] += providedText2[currenPos];
					    currenPos++;
				    }
				    else
				    { 
					    rows[i] += 'q';
					    currenPos++;
				    }
				    //------------------------------------------------------------
			    }
		    }
		    //Read down each column and add this text to our output text.
		    for (int i = 0; i < columnCount; i++)
		    {
			    for (int j = 0; j < columnSize; j++)
			    {
				    adjusted += rows[j][i];
			    }
		    }
		    return adjusted;
	    }
        //----------END Columnar Cipher-------------------------------------



        //----------Vigenere Cipher---------------------------------------
        /*This function cleans up whitespace in the key and encryptedWord, and then converts each letter
        in the key and encryptedWord to the index in which it is located in the alphabet*/
        private List<int> VigenereCipher()
        {
            //trim whitespace of plainText
            string trim_plainText = plainText.Trim();
            string remove_whitespace = "\\s+";
            string replace = "";
            string InText = Regex.Replace(trim_plainText, remove_whitespace, replace);

            //trim whitespace of keyWordInput
            string trim_keyWordInput = keyWordInput.Trim();
            string remove_wspace = "\\s+";
            string replaced = "";
            string KeyText = Regex.Replace(trim_keyWordInput, remove_wspace, replaced);

            //convert keyText and InText to char array
            char[] keyWord = KeyText.ToCharArray();
            char[] encryptWord = InText.ToCharArray();

            //get index in alphabet of each letter in key
            for (int i = 0; i < KeyText.Length; i++)
            {
                int keyIndex = Array.IndexOf(alphabet, keyWord[i]);
                keyInts.Add(keyIndex);
            }
            //get index in alphabet of each letter in encrypted word
            for (int i = 0; i < InText.Length; i++)
            {
                int encryptedIndex = Array.IndexOf(alphabet, encryptWord[i]);
                encryptedInts.Add(encryptedIndex);
            }

            List<int> returnedEncrypted = new List<int>(NewKeyIndex( keyInts, encryptedInts));
            return returnedEncrypted;
        }
        /*This function subtracts the index of the key from each letter in the encryptedWord. For each 
        letter in the encryption, it uses the number at the same index in key. Once it has reached the
        end of the key, it returns to the start of the key.*/
        private List<int> NewKeyIndex(List<int> keyInts, List<int> encryptedInts)
        {

            for (int i = 0, j = 0; i < encryptedInts.Count(); i++, j++)
            {//if it has reached the end of the key, go back to the start of key
                if (j > keyInts.Count() - 1)
                {
                    j = 0;
                }
                //subtract key index from encrypted index
                int newIndex = encryptedInts[i] - keyInts[j];

                //if newIndex if negative, add 26 to convert to correct letter in index
                if (newIndex < 0)
                {
                    newIndex = newIndex + 26;
                }

                decryptedInts.Add(newIndex);

            }
            return decryptedInts;
            //DecryptVigenere();

        }
        /*This function converts the numbers in decryptedInts back to letters, and then returns
        the solution*/
        public string DecryptVigenere()
        {           
            if(keyWordInput=="")
            {
                return "No Keyword. Not Vigenere.";
            }
            else 
            {
                List<int> returnedEncryptedFinal = new List<int>(VigenereCipher());
                string decryptedText = "";
                for (int i = 0; i < decryptedInts.Count(); i++)
                {
                    decryptedText += alphabet[returnedEncryptedFinal[i]];
                }

                return decryptedText;
            }


        }
        //----------END Vigenere Cipher-------------------------------------





	    //----------Permutation Cipher--------------------------------------
	    public string[] DecryptAnagram()
	    {
		    //Blatently reusing this chunk since it works
		    //Clean up cipherText for usability
		    string trim_plainText = plainText.Trim(); // remove all leading and trailing white-space characters
		    string remove_whitespace = "\\s+"; // find all mid string white-space characters with Regex
		    string replace = "";
		    string InText = Regex.Replace(trim_plainText, remove_whitespace, replace);            
		    InText = InText.ToLower(); // change all to lowercase
		    //char[] encTextArr = InText.ToCharArray();
		    //char[] decTextArr = new char[encTextArr.Length]; 
		    int counter = 0;
		    //int indexer = -1;
		    //int length = -1;
		    List<string> permutations = new List<string>();
		    List<string> possibleSolutions = new List<string>();
		    int temporary = -1;
		    string possiblePlainText = "";

		    //Checking for anagram lengths between 2 and 8 charachters
		    //Possible anagram is outside this length but 2-8 is most likely by far
		    //Inside the code x will be used interchangably with the number of collumns in the cipher
		    for (int x = 2; x < 8; x++)
		    {
			    //Ensure the guessed keyword length fits the ciphertext length
			    if(InText.Length%x == 0)
			    {
				    //Setting variable for number of rows in the cipher
				    int height = InText.Length/x;

                    //filling an array of apropriate length with the modified cipher text
                    char[,] tempArray = new char[x, height];
				    for (int a = 0; a<x; a++)
				    {
					    for (int b = 0; b< height; b++)
					    {
                            tempArray[a, b] = InText[counter];
						    counter++;
					    }
				    }

                    //Creating a string of numbers of length x to permute
                    StringBuilder sb = new StringBuilder();
                    sb.Append("");
				    for(int i = 0; i<x; i++)
                    {                    
                        sb.Append(i.ToString());
                    }
                    string toPermute = sb.ToString();
                    System.Diagnostics.Debug.WriteLine(toPermute);


                    //running permutation function
                    permute(permutations,toPermute, 0, x-1);
                
				    //running through all possible permutations
				    for(int y = 0; y<permutations.Count; y++)
				    {
					    //keeping track of current row chars are being grabbed from
					    for(int currentRow = 0; currentRow<height; currentRow++)
					    {
						    //making sure not to index out of bounds the permutation list, each string therein should be length x so use that as sentinal since it's easier
						    for(int currentColumn = 0; currentColumn<x; currentColumn++)
						    {
							    //getting the number value of the char held at a given point in the current permutation
							    temporary = (int)Char.GetNumericValue(permutations[y][currentColumn]);                            
                                //Adding the charachters in the permuted order to the string
                                possiblePlainText += tempArray[temporary, currentColumn];
						    }
					    }
					    possibleSolutions.Add(possiblePlainText);
					    possiblePlainText = "";
				    }
			    }
		    }

		    //Sorting for accuracy
		    string[] bulkPossibleDecryptions = possibleSolutions.ToArray();
		    string[] bulkDecryptions = WeightDigramTrigram(bulkPossibleDecryptions);

		    //Returning the titanic amount of possiblities
		    return bulkDecryptions;
	    }

	    //Permutation function with string, starting index, ending index respectfully
	    private static List<string> permute(List<string> permutations,String word, int s, int e)
	    {
		    if (s == e)
			    //returns a string of numbers to guide what collumns to draw from permutation wise
			    permutations.Add(word);
		    else
		    {
			    for (int i = s; i <= e; i++)
			    {
                   //word = swap(word, s, i);
                   word = swap(word, s, e);
                   permute(permutations,word, s + 1, e);
			       //word = swap(word, s, e);
			    }
		    }
                return permutations;
	    }

	    //Swap charachters at positions given
	    public static String swap(String a, int i, int j)
	    {
		    char temp;
		    char[] charArray = a.ToCharArray();
		    temp = charArray[i];
		    charArray[i] = charArray[j];
		    charArray[j] = temp;
		    string swapped = new string(charArray);
		    return swapped;
	    }

        //----------END Permutation Cipher---------------


        //----------Keyword Cipher---------------------------------------
        // similar to Caesar cipher, so this code is derived from the code for that.
        // Biggest difference is the keyword, so the shifting must be done differently
        // have to figure out the whole alphabet order, then shift the key and change InText
        public char[] KeyShift(char[] InText, char[] key, int shift)
        {
            // shift key
            char[] endKey = new char[key.Length]; // empty array for the key to use for this shift
            for (int i = 0; i < key.Length; i++)
            {
                if ((i + shift) < key.Length) // make sure the resulting index isn't over the key length
                {
                    endKey[i] = key[i + shift];
                } // end if
                else // if it is, loop back to the beginning of the key
                {
                    endKey[i] = key[(i + shift) - key.Length];
                } // end if
            } // end for

            // use shifted key to convert InText
            char[] outText = new char[InText.Length];
            for (int i = 0; i < InText.Length; i++)
            {
                for (int j = 0; j < key.Length; j++)
                {
                    if (InText[i] == key[j])
                    {
                        outText[i] = endKey[j];
                        break;
                    } // end if
                } // end for
            } // end for

            return outText;
        }
        public string[] DecryptKeyword(string plainText, string keyWordInput)
        {
            string[] bulkDecryptions = new string[25];
            // clean up plainText for usability
            string trim_plainText = plainText.Trim(); // remove all leading and trailing white space characters
            string remove_whitespace = "\\s+"; // find all mid string white-space characters with Regex
            string replace = "";
            string InText = Regex.Replace(trim_plainText, remove_whitespace, replace);
            InText = InText.ToLower(); // change all to lowercase


            // clean up keyWordInput as well
            string trim_keyWordInput = keyWordInput.Trim();
            string keyText = Regex.Replace(trim_keyWordInput, remove_whitespace, replace);
            keyText = keyText.ToLower(); // change to lowercase

            // convert text to character arrays
            char[] encTextArr = InText.ToCharArray();
            char[] decTextArr = new char[encTextArr.Length];
            char[] keyTextArr = keyText.ToCharArray();

            // create new alphabet based on the keyword
            char[] alphaKey = new char[26];
            //char[] tempAlpha = alphabet;
            // temporary alphabet, blank out used characters as we go
            char[] tempAlpha = new char[26];
            Array.Copy(alphabet, tempAlpha, 26);
            for (int i = 0; i < 26; i++)
            {
                if (i < keyTextArr.Length)
                { // start with keyword characters
                    // '.' signifies a stand-in for an already used character
                    // we want to process each unique letter in the keyword and alphabet only once
                    if (alphaKey[i] != '.')
                    {
                        alphaKey[i] = keyTextArr[i]; // add each letter of the keyword to the alphabet
                        for (int j = 0; j < 26; j++)
                        {
                            if (tempAlpha[j] == alphaKey[i])
                            {
                                tempAlpha[j] = '.'; // blank out character in tempAlpha
                            } // else, do nothing
                        } // end for
                        for (int j = i; j < keyTextArr.Length; j++)
                        {
                            if (keyTextArr[j] == alphaKey[i])
                            {
                                keyTextArr[j] = '.'; // blank out character in keyword as well
                            }
                        } // end for
                    } // else, do nothing
                }
                else // fill in the rest of the alphabet
                {
                    for (int j = 0; j < 26; j++)
                    {
                        if (tempAlpha[j] != '.')
                        {
                            alphaKey[i] = tempAlpha[j]; // add this character to the alphaKey
                            tempAlpha[j] = '.'; // blank character out
                        }// end if
                    } // end for
                }// end else
            }// end for


            // Need to check all 25 shifts
            // Decrypt uses negative values
            for (int i = 0; i < 25; i++)
            {
                int shift = i + 1;
                /*for (int j = 0; j < encTextArr.Length; j++)
                {
                    decTextArr[j] = KeyShift(encTextArr[j], shift);

                }*/
                decTextArr = KeyShift(encTextArr, alphaKey, shift);
                string potDecrypt = new string(decTextArr);
                bulkDecryptions[i] = potDecrypt;
            }

            string[] bulkDecryptionsShortened = new string[] { };
            bulkDecryptionsShortened = WeightDigramTrigram(bulkDecryptions);
            return bulkDecryptionsShortened;
        } // end DecryptKeyword
        public string[] PrintKeywordCipher()
        {
            return DecryptKeyword(plainText, keyWordInput);
        }
        //----------END Keyword Cipher-------------------------------------



        



        // Filter possible decryptions by trigram/digram
        private string[] WeightDigramTrigram(string[] inputStringArray)
        {
            string[] digrams = new string[] { "th", "in", "he", "er", "re", "on", "es", "an", "at", "ti" };
            string[] trigrams = new string[] { "the", "ing", "tha", "and", "hat", "ion", "ent", "you", "thi", "for" };
            // currently just find digram and trigrams            
            int[] weights = new int[inputStringArray.Length];
            for (int i = 0; i < inputStringArray.Length; i++)
            {
                for (int j = 0; j < digrams.Length; j++)
                {
                    if (inputStringArray[i].Contains(digrams[j]))
                        weights[i]++;
                    if (inputStringArray[i].Contains(trigrams[j]))
                        weights[i]++;
                }
            }
            string[] outputWeightedCiphers = new string[inputStringArray.Length];
            int temp = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                // will sort whole array eventually
                int highestWeight = weights.Max();
                if (highestWeight == 0) break;
                int IndexMax = Array.IndexOf(weights, highestWeight);
                outputWeightedCiphers[i] = inputStringArray[IndexMax];
                weights[IndexMax] = 0;
                temp = i;
            }
            temp++;
            string[] outputWeightedCiphersShortened = new string[temp];
            Array.Copy(outputWeightedCiphers, 0, outputWeightedCiphersShortened, 0, temp);
            return outputWeightedCiphersShortened;
        }







    }// end Decryption
}// end SecurityCourseProject namespace
