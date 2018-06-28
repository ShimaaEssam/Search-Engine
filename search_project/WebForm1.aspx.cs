using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Data.SqlClient;

namespace search_project
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
       // static string connstring = "Data Source=SHIMAA;Initial Catalog=IRdb;Integrated Security=True";
        static string connstring = "Data Source=localhost ;Initial Catalog = IRTable;Integrated Security=True";

        struct Inverted_index
        {

            public List<string> DOCID;
            public List<int> FREQ;
            public List<List<string>> POS;
        }

        SortedDictionary<string, Inverted_index> InvertedIndex_Dic = new SortedDictionary<string, Inverted_index>();
        List<string> UrlList = new List<string>();



        struct Inverted_index2
        {

            public List<string> DOCID2;
            public List<int> FREQ2;
            public List<List<string>> POS2;
        }

        SortedDictionary<string, Inverted_index> InvertedIndex_Dic2 = new SortedDictionary<string, Inverted_index>(); // 3shan arg3 l data mn ldatabase
       

        private void Stemming()
        {

            SqlConnection conn = new SqlConnection(connstring);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select Term,docid,positions from IRdb.dbo.[Inverted_Index]", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string term = (string)reader["Term"];
                    string stemming_term = steming_fun(term);
                    insert_stemming_term(stemming_term);

                }
                reader.Close();
            }

            catch (SqlException)
            {
            }
            conn.Close();
        }
        private string steming_fun(string term)
        {
            Porter2 s = new Porter2();
            string new_term = s.stem(term);
            return new_term;
        }

        private void insert_stemming_term(string new_term)
        {
            SqlConnection conn = new SqlConnection(connstring);

            try
            {
                conn.Open();
               SqlCommand cmd2 = new SqlCommand("UPDATE [IRdb].[dbo].[final_table] SET Term= @term ", conn);
                cmd2.Parameters.AddWithValue("@term", new_term);
                cmd2.ExecuteNonQuery();
            }
            catch (SqlException)
            {  }
            conn.Close();
        }


        protected void Inverted_Index(object sender, EventArgs e)
        {


            SqlConnection conn = new SqlConnection(connstring);
            try
            {

               
                    conn.Open();

                    //SqlCommand cmd = new SqlCommand("select new_terms,doc_id,position from IRdb.dbo.newterm  ", conn);

                    SqlCommand cmd = new SqlCommand("select Term,docid , positions from [IRdb].[dbo].[newterm]  ", conn);
                    SqlDataReader reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        //c++;
                        //if (c == 10)
                        //{
                        //    break;
                        //}
                        string stemming_term = (string)reader["Term"]; // mn l table after stemming
                        string document_id = (string)reader["docid"];
                        string position = (string)reader["positions"];


                        if (InvertedIndex_Dic.ContainsKey(stemming_term))
                        {
                            // da lw l term mwgood , Hat check 3l doc id
                            // lw howa howa  hazwad el freq bt3to
                            //lw la2 yb2a ha3ml add lel doc id f nafs l makan(index) wl freq bt3to hazwdha ganb l freq brdo fe nfs el index


                            if (InvertedIndex_Dic[stemming_term.ToString()].DOCID.Contains(document_id))
                            {
                                int ind = InvertedIndex_Dic[stemming_term.ToString()].DOCID.IndexOf(document_id);
                                InvertedIndex_Dic[stemming_term].FREQ[ind]++;
                                InvertedIndex_Dic[stemming_term].POS[ind].Add(position);

                            }
                            else   // lw l stemmed term msh mwgood fl dic fa ha3mlo add
                            {

                                InvertedIndex_Dic[stemming_term.ToString()].DOCID.Add(document_id);
                                InvertedIndex_Dic[stemming_term].FREQ.Add(1);
                                List<string> newPos = new List<string>();
                                newPos.Add(position);
                                InvertedIndex_Dic[stemming_term].POS.Add(newPos);

                            }



                        }

                        else  // lw msh mwgood h3ml lel term w add le doc id wl freq
                        {
                            var ii = new Inverted_index();

                            ii.DOCID = new List<string>();
                            ii.FREQ = new List<int>();
                            ii.POS = new List<List<string>>();
                            ii.DOCID.Add(document_id);
                            ii.FREQ.Add(1);
                            List<string> doc_id_pos = new List<string>();
                            doc_id_pos.Add(position);
                            ii.POS.Add(doc_id_pos);

                            InvertedIndex_Dic.Add(stemming_term, ii);

                        }

                    } // while

                    conn.Close();


                    // } // end for

                    int test = 0;
                    foreach (string s in InvertedIndex_Dic.Keys)
                    {
                        string t = "", f = "", p = "", docString = "";

                        t = s;

                        for (int doc_i = 0; doc_i < InvertedIndex_Dic[s].DOCID.Count; doc_i++)
                        {
                            if (InvertedIndex_Dic[s].DOCID.Count != 1)
                            {
                                docString += InvertedIndex_Dic[s].DOCID[doc_i] + ",";

                            }
                            else
                            {
                                docString = InvertedIndex_Dic[s].DOCID[doc_i];

                            }

                        }
                        for (int freq_i = 0; freq_i < InvertedIndex_Dic[s].FREQ.Count; freq_i++)
                        {
                            if (InvertedIndex_Dic[s].FREQ.Count != 1)
                            {
                                f += InvertedIndex_Dic[s].FREQ[freq_i] + ",";

                            }
                            else
                            {
                                f = InvertedIndex_Dic[s].FREQ[freq_i].ToString();

                            }

                        }

                        for (int pos_i = 0; pos_i < InvertedIndex_Dic[s].POS.Count; pos_i++)
                        {

                            for (int lists = 0; lists < InvertedIndex_Dic[s].POS[pos_i].Count; lists++)
                            {
                                if (InvertedIndex_Dic[s].POS[pos_i].Count == 1)
                                {
                                    p += InvertedIndex_Dic[s].POS[pos_i][lists];

                                }
                                else
                                {
                                    p += InvertedIndex_Dic[s].POS[pos_i][lists] + ",";

                                }

                            }
                            if (InvertedIndex_Dic[s].POS.Count != 1)
                            {
                                p += "@";
                            }

                        }

                        insert_invertedIndex(t, docString, f, p);
                    } // foreach

                    int test2 = 0;



                
            }


    
            catch (SqlException)
            {

            }
            conn.Close();

        }

        
        private void insert_invertedIndex(string new_term, string document_id, string freq, string position)
        {
            SqlConnection conn = new SqlConnection(connstring);

            try
            {
                conn.Open();

                SqlCommand cmd2 = new SqlCommand("insert into [IRdb].[dbo].[Inverted_Index] (Term,docid,Frequency,positions) values(@Term,@docid,@Frequency,@positions)", conn);
              //   SqlCommand cmd2 = new SqlCommand("UPDATE  [IRdb].[dbo].[Inverted_Index] SET [Term]= @term ,[docid]=@docid , [Frequency]=@frequency ,[positions]=@positions where  [Frequency] = NULL", conn);


                cmd2.Parameters.AddWithValue("@term", new_term);
                cmd2.Parameters.AddWithValue("@docid", document_id);
                cmd2.Parameters.AddWithValue("@frequency", freq);
                cmd2.Parameters.AddWithValue("@positions", position);


                cmd2.ExecuteNonQuery();

            }
            catch (SqlException)
            {  }
            conn.Close();
        }

        protected void Bi_Gram(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(connstring);
            try
            {
                conn.Open();

                // SqlCommand cmd = new SqlCommand("select new_terms from IRdb.dbo.newterm", conn);
                SqlCommand cmd = new SqlCommand("select Term from [IRdb].[dbo].[copyofterms]", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                string Inverted_term = "";
                string Bi_gram_Term = "";
                Dictionary<string, string> K_grams = new Dictionary<string, string>();
                List<string> terms_list = new List<string>();
                List<string> bigrams_list = new List<string>();
                int ind = 0;
                int g = 0;
                #region reader
                while (reader.Read())
                {
                    Inverted_term = (string)reader["Term"];
                    terms_list.Add(Inverted_term);

                    Bi_gram_Term = "$" + Inverted_term + "$"; // to add $ at the start and at the end of the word
                    char[] array = Bi_gram_Term.ToCharArray();

                    for (int i = 0; i < array.Length-1 ; i++)
                    {

                        bigrams_list.Add(array[i].ToString() + array[i + 1].ToString()); // res : feha kol bi grams bt3t kol l terms 
                        //   g++;
                    }

                    // ind++;

                } // while

                reader.Close();

                #endregion

                string temp = "";
                for (int t = 0; t < terms_list.Count; t++)
                {
                    for (int b = 0; b < bigrams_list.Count; b++)
                    {
                        #region $..
                        if (bigrams_list[b].ElementAt(0) == '$')
                        {
                            if (bigrams_list[b].ElementAt(1).ToString() == terms_list[t].ElementAt(0).ToString())
                            {
                                temp = "";
                                temp = terms_list[t] + ",";
                                if (K_grams.ContainsKey(bigrams_list[b]))
                                {
                                    if (!K_grams[bigrams_list[b]].Contains(temp))
                                    {
                                        K_grams[bigrams_list[b]] += temp;
                                    }

                                }
                                else
                                {
                                    K_grams.Add(bigrams_list[b].ToString(), temp);
                                }

                            }
                        }
                        #endregion
                        //----------------------
                        #region ..$

                        else if (bigrams_list[b].ElementAt(1) == '$')
                        {
                            if (terms_list[t].EndsWith(bigrams_list[b].ElementAt(0).ToString()))
                            {
                                temp = "";
                                temp = terms_list[t] + ",";
                                if (K_grams.ContainsKey(bigrams_list[b]))
                                {
                                    if (!K_grams[bigrams_list[b]].Contains(temp))
                                    {
                                        K_grams[bigrams_list[b]] += temp;
                                    }

                                }
                                else
                                {
                                    K_grams.Add(bigrams_list[b].ToString(), temp);
                                }

                            }
                        }
                        #endregion
                        //----------------------
                        #region .. between ..
                        else
                        {
                            if (terms_list[t].ToString().Contains(bigrams_list[b]) == true)
                            {
                                temp = "";
                                temp = terms_list[t] + ",";
                                if (K_grams.ContainsKey(bigrams_list[b]))
                                {
                                    if (!K_grams[bigrams_list[b]].Contains(temp))
                                    {
                                        K_grams[bigrams_list[b]] += temp;
                                    }

                                }
                                else
                                {
                                    K_grams.Add(bigrams_list[b].ToString(), temp);
                                }

                            }
                        }
                        #endregion

                    } // for of b

                } // for of t
                string gram = "", te = "";
                foreach (KeyValuePair<string, string> pair in K_grams)
                {
                    gram = pair.Key;
                    te = pair.Value;
                    insert_Bigram_Table(gram, te);
                }
                //MessageBox.Show("bi gram added");

            }

            catch (SqlException)
            {
            }

            conn.Close();

        }

        private void insert_Bigram_Table(string k_gram, string term)
        {
            SqlConnection conn = new SqlConnection(connstring);

            try
            {
                conn.Open();

                SqlCommand cmd2 = new SqlCommand("insert into [IRdb].[dbo].[bigram](gram,term) values(@k_gram, @terms)", conn);
                cmd2.Parameters.AddWithValue("@k_gram", k_gram);
                cmd2.Parameters.AddWithValue("@terms", term);


                cmd2.ExecuteNonQuery();
            }
            catch (SqlException)
            {  }
            conn.Close();
        }

        protected void Search(object sender, EventArgs e)
        {


            SqlConnection conn = new SqlConnection(connstring);
            try
            {

                conn.Open();


                SqlCommand cmd = new SqlCommand("select Term,docid , positions from IRTable.dbo.Updated_InvertedIndex ", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                var iiObj = new Inverted_index();

                iiObj.DOCID = new List<string>();
                iiObj.FREQ = new List<int>();
                iiObj.POS = new List<List<string>>();


                while (reader.Read())
                {

                    string invertedTerm = (string)reader["Term"]; // mn l table after stemming
                    string document_id = (string)reader["docid"];
                    string freq = (string)reader["Frequency"];
                    string position = (string)reader["positions"];

                 
                    string[] docid = document_id.Split(',');
                    string[] frq = freq.Split(',');
                    iiObj.DOCID.AddRange(docid);
                    for (int l = 0; l < frq.Length; l++)
                    {
                        int f = int.Parse(frq[l]);
                        iiObj.FREQ.Add(f);
                    }

                    string[] poslist1 = position.Split('@');
                    for(int y=0;y<poslist1.Length;y++){
                    List<string> pos2 = new List<string>();
                     pos2.AddRange(poslist1[y].Split(','));
                     iiObj.POS.Add(pos2);
                    }
                    
                    

                }//while

               }//try
            catch (SqlException) 
            {

            }










                    if (txt_searchQuery.Text == "")
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Enter Your Search Query Please!')", true);

                    }
                    else
                    {
                        if (RadioButton1.Checked == true)
                        {


                        }
                        if (RadioButton2.Checked == true)
                        {

                        }
                        else // search 3ady ya2ma multi search aw exact search
                        {
                            if (txt_searchQuery.Text[0] == '"' && txt_searchQuery.Text[txt_searchQuery.Text.Length - 1] == '"')
                            {
                                #region exact_search
                                {
                                    List<string> terms_without_stops = new List<string>();

                                    IEnumerable<String> un_id;

                                    string query = txt_searchQuery.Text.ToString();
                                   


                                    char[] delimiters = new char[] { '#', '=', '\'', '\"', ':', '×', ';', ',', '÷', '.', '\\', '?', ' ', '/', '<', '>', 
                    '&', '!', '(', ')', '~', '@', '$', '%', '^', '*', ']', '[', '+', '_', '-', '|', '}', '{' };

                                    String stop_word = "a about above across after afterwards again against all almost alone along already also although always am among amongst amoungst amount an and another any anyhow anyone anything anyway anywhere are around as at back be became because become becomes becoming been before beforehand behind being below beside besides between beyond bill both bottom but by call can cannot cant co computer con could couldnt cry de describe detail do done down due during each eg eight either eleven else elsewhere empty enough etc even ever every everyone everything everywhere except few fifteen fify fill find fire first five for former formerly forty found four from front full further get give go had has hasnt have he hence her here hereafter hereby herein hereupon hers herself him himself his how however hundred i ie if in inc indeed interest into is it its it's i'm itself keep last latter latterly least less ltd made many may me meanwhile might mill mine more moreover most mostly move much must my mysel name namely neither never nevertheless next nine no nobody none noon nor not nothing now nowhere of off often on once one only onto or other others otherwis our ours ourselves out over own part per perhaps please put rather re same see seem seemed seemin seems serious severa she should show side since sincere six sixty so some somehow someone something sometime sometimes somewhere still such syste take ten than that the their them themselves then thence there thereafter thereby therefore therein thereupon these they thick thin this third this those though three through throughout thru thus to together too top toward towards twelve twenty two un under until up upon us very via was we well were what whatever when whence whenever where whereafter whereas whereby wherein whereupon wherever whether which while whither who whoever whole whom whose why will with within without would yet you your yours yourself yourselves";
                                    string[] stopword = stop_word.Split(' ');

                                    query = query.ToLower();
                                    for (int i = 0; i < delimiters.Length; i++)
                                    {
                                        query = query.Replace(delimiters[i], ' ');
                                    }
                                    //term with stopword 
                                    string[] terms = query.Split(' ');

                                    string Two_Terms1 = "";
                                    string Two_Terms2 = "";

                                    List<string> pos1 = new List<string>();
                                    List<string> pos2 = new List<string>();
                                    SortedSet<int> Unq = new SortedSet<int>();
                                    Dictionary<string, List<int>> Messo = new Dictionary<string, List<int>>();
                                    List<int> minVal = new List<int>();
                                    List<int> freqPos = new List<int>();


                                    //----------------------------------------------
                                    #region GetFreq Of Positions


                                    for (int i = 0; i < terms.Length - 1; i++)
                                    {
                                        Two_Terms1 = terms[i];
                                        Two_Terms2 = terms[i + 1];

                                        //remove stop words
                                        if (!stopword.Contains(Two_Terms1) && Two_Terms1 != " " && !stopword.Contains(Two_Terms2) && Two_Terms2 != " ")
                                        {

                                            Porter2 s = new Porter2();
                                            Two_Terms1 = s.stem(Two_Terms1);
                                            Two_Terms2 = s.stem(Two_Terms2);


                                            un_id = InvertedIndex_Dic[Two_Terms1].DOCID.Intersect(InvertedIndex_Dic[Two_Terms2].DOCID); // intersect between doc


                                            foreach (string UnId in un_id)
                                            {
                                                Unq.Add(int.Parse(UnId));

                                                freqPos = new List<int>();
                                                int c = 0;
                                                int ind1 = InvertedIndex_Dic[Two_Terms1].DOCID.IndexOf(UnId); // index of unID
                                                int ind2 = InvertedIndex_Dic[Two_Terms2].DOCID.IndexOf(UnId);

                                                // bageb l indices 3shan aroo7 ageeb l positions bt3t l indices de
                                                foreach (String pos1_ in InvertedIndex_Dic[Two_Terms1].POS[ind1]) // pos of word at docid 1 ( unID)
                                                {
                                                    foreach (String pos2_ in InvertedIndex_Dic[Two_Terms2].POS[ind2])
                                                    {
                                                        if (int.Parse(pos2_) - int.Parse(pos1_) == 1)
                                                        {
                                                            c++;
                                                            break;
                                                        }
                                                    }
                                                }
                                                freqPos.Add(c);
                                                if (!Messo.ContainsKey(UnId))
                                                {
                                                    Messo.Add(UnId, freqPos);
                                                }
                                                else
                                                {
                                                    Messo[UnId].AddRange(freqPos);
                                                }



                                            } // each

                                        }//if

                                    }//for i
                                    #endregion
                                    //-------------------------------------------

                                    int max = 0;
                                    int DOcUmEntId = 0;
                                    int index = 0;
                                    UrlList.Clear();
                                    minVal.Clear();
                                    for (int min = 0; min < Unq.Count; min++)
                                    {

                                        minVal.Add(Messo[Unq.ElementAt(min).ToString()].Min());

                                    }
                                    int minvalueCount = minVal.Count;
                                    for (int x = 0; x < minvalueCount; x++)
                                    {
                                        max = minVal.Max();   //haroo7 ageeb l docid of max number da mn  Messoo Dic

                                        if (max == 0)
                                        {
                                            Get_URLs(max);

                                        }
                                        else
                                        {
                                            index = minVal.IndexOf(max);
                                            DOcUmEntId = int.Parse(Messo.Keys.ElementAt(index));
                                            Get_URLs(DOcUmEntId);
                                            minVal.RemoveAt(minVal.IndexOf(max));
                                            minVal.Insert(index, -100000);

                                        }
                                        //var items = new List<int> { 8, 5, 9 };
                                        //for (int t = 0; t < items.Count; t++)
                                        //{
                                        //    int test = items.IndexOf(items.Max());
                                        //    items.RemoveAt(test);
                                        //    items.Insert(test, -100000);

                                        //}


                                    }
                                    add_links_to_text(UrlList);
                                    UrlList.Clear();
                                }
                                #endregion

                            }
                            else
                            {
                                #region multi_word_search

                                {

                                    List<string> terms_without_stops = new List<string>();

                                    IEnumerable<String> un_id;

                                    string query = txt_searchQuery.Text.ToString();
                                    char[] delimiters = new char[] { '#', '=', '\'', '\"', ':', '×', ';', ',', '÷', '.', '\\', '?', ' ', '/', '<', '>', 
                    '&', '!', '(', ')', '~', '@', '$', '%', '^', '*', ']', '[', '+', '_', '-', '|', '}', '{' };

                                    String stop_word = "a about above across after afterwards again against all almost alone along already also although always am among amongst amoungst amount an and another any anyhow anyone anything anyway anywhere are around as at back be became because become becomes becoming been before beforehand behind being below beside besides between beyond bill both bottom but by call can cannot cant co computer con could couldnt cry de describe detail do done down due during each eg eight either eleven else elsewhere empty enough etc even ever every everyone everything everywhere except few fifteen fify fill find fire first five for former formerly forty found four from front full further get give go had has hasnt have he hence her here hereafter hereby herein hereupon hers herself him himself his how however hundred i ie if in inc indeed interest into is it its it's i'm itself keep last latter latterly least less ltd made many may me meanwhile might mill mine more moreover most mostly move much must my mysel name namely neither never nevertheless next nine no nobody none noon nor not nothing now nowhere of off often on once one only onto or other others otherwis our ours ourselves out over own part per perhaps please put rather re same see seem seemed seemin seems serious severa she should show side since sincere six sixty so some somehow someone something sometime sometimes somewhere still such syste take ten than that the their them themselves then thence there thereafter thereby therefore therein thereupon these they thick thin this third this those though three through throughout thru thus to together too top toward towards twelve twenty two un under until up upon us very via was we well were what whatever when whence whenever where whereafter whereas whereby wherein whereupon wherever whether which while whither who whoever whole whom whose why will with within without would yet you your yours yourself yourselves";
                                    string[] stopword = stop_word.Split(' ');

                                    query = query.ToLower();
                                    for (int i = 0; i < delimiters.Length; i++)
                                    {
                                        query = query.Replace(delimiters[i], ' ');
                                    }
                                    //term with stopword 
                                    string[] terms = query.Split(' ');

                                    string Two_Terms1 = "";
                                    string Two_Terms2 = "";

                                    List<string> pos1 = new List<string>();
                                    List<string> pos2 = new List<string>();
                                    SortedSet<int> Unq = new SortedSet<int>();
                                    Dictionary<string, List<int>> Messo = new Dictionary<string, List<int>>();
                                    List<int> PosDifference = new List<int>();
                                    List<int> MinValues = new List<int>();

                                    List<List<int>> MinValuesList = new List<List<int>>();
                                    //----------------------------------------------
                                    if (terms.Length > 1) // more than one word
                                    {
                                        #region GetFreq Of Positions


                                        for (int i = 0; i < terms.Length - 1; i++)
                                        {
                                            int c = 0;
                                            int min = 0;
                                            int sum2 = 0;
                                            Two_Terms1 = terms[i];
                                            Two_Terms2 = terms[i + 1];

                                            //remove stop words
                                            if (!stopword.Contains(Two_Terms1) && Two_Terms1 != " " && !stopword.Contains(Two_Terms2) && Two_Terms2 != " ")
                                            {

                                                Porter2 s = new Porter2();
                                                Two_Terms1 = s.stem(Two_Terms1);
                                                Two_Terms2 = s.stem(Two_Terms2);



                                                un_id = InvertedIndex_Dic[Two_Terms1].DOCID.Intersect(InvertedIndex_Dic[Two_Terms2].DOCID);


                                                foreach (string UnId in un_id)
                                                {
                                                    Unq.Add(int.Parse(UnId));

                                                    PosDifference = new List<int>();

                                                    int ind1 = InvertedIndex_Dic[Two_Terms1].DOCID.IndexOf(UnId); // index of unID
                                                    int ind2 = InvertedIndex_Dic[Two_Terms2].DOCID.IndexOf(UnId);

                                                    // bageb l indices 3shan aroo7 ageeb l positions bt3t l indices de
                                                    foreach (String pos1_ in InvertedIndex_Dic[Two_Terms1].POS[ind1]) // pos of word at docid 1 ( unID)
                                                    {
                                                        foreach (String pos2_ in InvertedIndex_Dic[Two_Terms2].POS[ind2])
                                                        {

                                                            PosDifference.Add(Math.Abs(int.Parse(pos2_) - int.Parse(pos1_)));
                                                        }
                                                    }
                                                    min = PosDifference.Min();
                                                    List<int> dd = new List<int>();
                                                    dd.Add(min);
                                                    if (MinValuesList.Count > 1)
                                                    {
                                                        MinValuesList.ElementAt(c).AddRange(dd);

                                                    }
                                                    else
                                                    {
                                                        MinValuesList.Add(dd);

                                                    }

                                                    ////// hageb l min bt3t l list awl mara w b3den ageeb l min tany mara 
                                                    if (!Messo.ContainsKey(UnId))
                                                    {
                                                        Messo.Add(UnId, PosDifference);

                                                    }
                                                    else
                                                    {
                                                        Messo[UnId].AddRange(PosDifference);
                                                    }

                                                    c++;

                                                } // each

                                            }//if


                                        }//for i
                                        #endregion
                                    }
                                    else  // one word
                                    {
                                        #region GetFreq Of Positions

                                        int c = 0;
                                        int min = 0;
                                        int sum2 = 0;
                                        Two_Terms1 = terms[0];

                                        //remove stop words
                                        if (!stopword.Contains(Two_Terms1) && Two_Terms1 != " ")
                                        {

                                            Porter2 s = new Porter2();
                                            Two_Terms1 = s.stem(Two_Terms1);

                                            un_id = InvertedIndex_Dic[Two_Terms1].DOCID;


                                            foreach (string UnId in un_id)
                                            {
                                                Unq.Add(int.Parse(UnId)); // contains all docid

                                                Get_URLs(int.Parse(UnId));

                                            }
                                        #endregion // one word

                                        }
                                        add_links_to_text(UrlList);
                                        UrlList.Clear();

                                    }
                                }
                                #endregion
                            }


                        } // else searhc 3ady


                    } // else eno maknsh wa7ed mn l buttons checked
                }
           
        


        private List<string> Get_URLs(int InputID)
        {

            string url = "";

            SqlConnection conn = new SqlConnection(connstring);
            conn.Open();


            SqlCommand cmd = new SqlCommand("select [linkUrl] from [IRTable].[dbo].[AllPages] where [id] = '" + InputID + "' ", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                url = (string)reader["linkUrl"];
                UrlList.Add(url);
            }

            if (UrlList.Count == 0)
            {
                UrlList.Add("No Match!");
                return UrlList;
            }
            else
            {
                return UrlList;

            }
            reader.Close();
            conn.Close();
        }

      
        private void add_links_to_text(List<string> links)
        {

            for( int m =0 ; m < links.Count ; m++)
            {
                Result.Text +=  links[m]+ "/n" ;
            }
        }

       // public int i { get; set; }

        private void Extract()
        {
            char[] delimiters = new char[] { '#', '=', '\'', '\"', ':', '×', ';', ',', '÷', '.', '\\', '?', ' ', '/', '<', '>', 
                '&', '!', '(', ')', '~', '@', '$', '%', '^', '*', ']', '[', '+', '_', '-', '|', '}', '{' };
            
            String stop_term = "a about above across after afterwards again against all almost alone along already also although always am among amongst amoungst amount an and another any anyhow anyone anything anyway anywhere are around as at back be became because become becomes becoming been before beforehand behind being below beside besides between beyond bill both bottom but by call can cannot cant co computer con could couldnt cry de describe detail do done down due during each eg eight either eleven else elsewhere empty enough etc even ever every everyone everything everywhere except few fifteen fify fill find fire first five for former formerly forty found four from front full further get give go had has hasnt have he hence her here hereafter hereby herein hereupon hers herself him himself his how however hundred i ie if in inc indeed interest into is it its it's i'm itself keep last latter latterly least less ltd made many may me meanwhile might mill mine more moreover most mostly move much must my mysel name namely neither never nevertheless next nine no nobody none noon nor not nothing now nowhere of off often on once one only onto or other others otherwis our ours ourselves out over own part per perhaps please put rather re same see seem seemed seemin seems serious severa she should show side since sincere six sixty so some somehow someone something sometime sometimes somewhere still such syste take ten than that the their them themselves then thence there thereafter thereby therefore therein thereupon these they thick thin this third this those though three through throughout thru thus to together too top toward towards twelve twenty two un under until up upon us very via was we well were what whatever when whence whenever where whereafter whereas whereby wherein whereupon wherever whether which while whither who whoever whole whom whose why will with within without would yet you your yours yourself yourselves";

            SqlConnection conn = new SqlConnection(connstring);
            try
            {
                string[] stopterm = stop_term.Split(' ');
                conn.Open();
                SqlCommand cmd = new SqlCommand("select id,mycontent from IRdb.dbo.AllPages", conn);


                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int doc_id = (int)reader["id"];
                    string text = (string)reader["mycontent"];
                    // string content = text.Trim();
                    //	CaseFolding
                    text = text.ToLower();
                    // replace el delemeters b space w split 3and el space w y5aznha f term array
                    for (int i = 0; i < delimiters.Length; i++)
                    {
                        text = text.Replace(delimiters[i], ' ');
                    }
                    //term with stopterm 
                    string[] terms = text.Split(' ');

                    for (int i = 0; i < terms.Length; i++)
                    { 
                        //remove stop terms
                        if (!stopterm.Contains(terms[i]) && terms[i] != "")
                        {
                            // (i+1) -> hna bnzawed wa7ed 3la el i 3l4an el pos maybda24 m zeroo     
                            insert(terms[i], doc_id, (i+1));

                        }

                    }
                

                }
                reader.Close();
            }
            catch (SqlException)
            {
                
            }
            conn.Close();
        }
        public static void insert(string t, int docid, int pos)
        {
            SqlConnection conn = new SqlConnection(connstring);

            try
            {
                conn.Open();
                // SqlCommand cmd2 = new SqlCommand("insert into [IRTable].[dbo].[_Inverted_Index] (Term,docid,positions) values(@term,@docid,@indexid)", conn);

                SqlCommand cmd2 = new SqlCommand("insert into [IRdb].[dbo].[Inverted_Index] (Term,docid,positions) values(@term,@docid,@indexid)", conn);
                cmd2.Parameters.AddWithValue("@term", t);
                cmd2.Parameters.AddWithValue("@docid", docid);
                cmd2.Parameters.AddWithValue("@indexid", pos);

                cmd2.ExecuteNonQuery();
            }
            catch (SqlException)
            { }
            conn.Close();
        }

        private void Soundex() 
            
        {

            SqlConnection conn = new SqlConnection(connstring);
            try
            {
                conn.Open();
                //for (int c = 44690; c < 49690; c++)
                //{


                SqlCommand cmd = new SqlCommand("select   [Term] from  [IRdb].[dbo].[copyofterms] ", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                string term = "";
                string soundex_Term = "";
                Dictionary<string, string> soundex_dic = new Dictionary<string, string>();
                while (reader.Read())
                {
                    term = (string)reader["term"];
                    term = term.ToUpper();
                    //bna5ed awel 7arf w ba3deen bnm4y 3la ba2y el kelma w bn3'atar el 7roof b el numbers elly btsawehaaa 
                    soundex_Term += term[0];
                    for (int i = 1; i < term.Length; i++)
                    {
                        if ((term[i] == 'A') || (term[i] == 'E') || (term[i] == 'I') || (term[i] == 'O') || (term[i] == 'U') || (term[i] == 'H') || (term[i] == 'W') || (term[i] == 'Y'))
                        {
                            soundex_Term += "0";
                        }
                        else if ((term[i] == 'B') || (term[i] == 'F') || (term[i] == 'P') || (term[i] == 'V'))
                        {
                            soundex_Term += "1";
                        }
                        else if ((term[i] == 'C') || (term[i] == 'G') || (term[i] == 'J') || (term[i] == 'K') || (term[i] == 'Q') || (term[i] == 'S') || (term[i] == 'X') || (term[i] == 'Z'))
                        {
                            soundex_Term += "2";
                        }
                        else if ((term[i] == 'D') || (term[i] == 'T'))
                        {
                            soundex_Term += "3";
                        }
                        else if (term[i] == 'L')
                        {
                            soundex_Term += "4";
                        }
                        else if ((term[i] == 'M') || (term[i] == 'N'))
                        {
                            soundex_Term += "5";
                        }
                        else if (term[i] == 'R')
                        {
                            soundex_Term += "6";
                        }
                        else
                        {
                            continue;
                        }


                    }//for end
                    string sounding_result = "";
                    //bna5ed awel 7arf bardoo w bnm4y 3la ba2y el 7roof nchek  el zeroos w law nafs el 7arf mtkarar marteen wra ba3d mabna5dhom4
                    // bna5ed arba3 7roof bas
                    sounding_result += soundex_Term[0];
                    for (int k = 1; k < soundex_Term.Length; k++)
                    {
                        if (soundex_Term[k] != 0 || soundex_Term[k] != soundex_Term[k + 1])
                        {
                            sounding_result += soundex_Term[k];
                        }
                        if (sounding_result.Length == 4)
                            break;

                    }
                    soundex_Term = "";
                    // law el chars a2al mn 4 bnzaed zeroo
                    while (sounding_result.Length < 4)
                    {
                        sounding_result += "0";
                    }
                    
                    if (soundex_dic.ContainsKey(sounding_result))
                    {
                        // bn4eek el term mawgood wla la 
                        // bna5ed el terms kolha f string w ba3deen bn3mlha split b el ',' w ba5azenha f array w bachek el term maugood f el array da wla la !
                        string check_term = soundex_dic[sounding_result];
                        string[] check_array = check_term.Split(',');
                        if (!check_array.Contains(term))
                        {
                            soundex_dic[sounding_result] += ',' + term;
                        }
                    }
                    else
                    {
                        soundex_dic.Add(sounding_result, term);
                    }

                }//while end
                foreach (KeyValuePair<string, string> pair in soundex_dic)
                {
                    string sound = pair.Key;
                    string te = pair.Value;
                    insert_sound_Table(sound, te);
                }
                reader.Close();
                //try end

            }

            catch (SqlException)
            {

            }
            conn.Close();

        }

        private void insert_sound_Table(string sound, string te)
        {
            SqlConnection conn = new SqlConnection(connstring);

            try
            {
                conn.Open();

                SqlCommand cmd2 = new SqlCommand("insert into [IRdb].[dbo].[Soundex](soundex,term) values(@sound, @terms)", conn);
                cmd2.Parameters.AddWithValue("@sound", sound);
                cmd2.Parameters.AddWithValue("@terms", te);


                cmd2.ExecuteNonQuery();
            }
            catch (SqlException)
            { }
            conn.Close();
        }

    }
}