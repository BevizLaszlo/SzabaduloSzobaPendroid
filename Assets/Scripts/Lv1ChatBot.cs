using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class Lv1ChatBot : MonoBehaviour
{
    public static List<string> ReadBooks { get; set; } = new List<string>();
    
    private static readonly string aiName = "AI";
    private readonly string playerPrompt = ">";
    
    [SerializeField] private TMP_Text textField;
    [SerializeField] private TMP_InputField inputField;
    public Dictionary<string, (string CorrectAnswer, string[] AnswerChoices, string Hint, string Book)> Questions { get; set; }
    public Dictionary<string[], Action> CommandHandlers { get; set; }
    public List<string> HelpResponses { get; set; }
    public Dictionary<string, string> GreetingResponses { get; set; }

    public (string Question, string CorrectAnswer, string[] AnswerChoices, string Hint, string Book) CurrentQuestion { get; set; }
    public string CurrentQuestionAnswer { get; set; }
    public bool DebugMode { get; set; }
    public List<string> Profanities { get; set; }
    
    private string LastOutput;

    public static void setBookRead(string bookName)
    {
        if (!ReadBooks.Contains(bookName))
            ReadBooks.Add(bookName);
    }
    
    public void Awake()
    {
        Questions = new Dictionary<string, (string, string[], string, string)>
        {
            { "Ő az, ki éjszaka ragyog, de napközben nem látod. Ki vagy mi ő?", ("Hold", new[] { "Nap", "Csillag", "Hold", "Éjjeli fény" }, "Nappal elrejtőzik, mégis minden éjjel uralkodik.", "Etir - A Hold küldötte") },
            { "Milyen drágakövet lopott el Feiht Zuikoppy udvarából?", ("Rubint", new[] { "Zafir", "Smaragd", "Rubint", "Gyémánt" }, "Piros, mint a vér... és ugyanolyan értékes.", "Qwadty - Az Arany Menyét") },
            { "Ez a növény a csontokat forrasztja, tüskéi szúrósak és hidegben él. Mi a neve?", ("Enarc", new[] { "Tfiws", "Enarc", "Niar", "Griff" }, "Fagyos tüskék őrzik, és csak a bátrak érhetik el gyümölcsét.", "Wert - Termetes egyedek területeinken (Növények)") },
            { "Melyik növény világít a sötét barlangokban, reménysugarat hozva az elveszetteknek?", ("Tfiws", new[] { "Tfiws", "Enarc", "Triton", "Zarvek" }, "Fénye vezet a mélységben, de csak azoknak, akik nem félnek a sötéttől.", "Wert - Termetes egyedek területeinken (Növények)") },
            { "Ki segítette Wertet bosszúja során, cserébe a lelkéért?", ("Hkrojklo", new[] { "Hkrojklo", "Feiht", "Gruhklo", "Rahn" }, "Ő, aki a halált és a szerelmet egyaránt uralja.", "Tsol - A vérző szív harcosa") },
            { "Mi ragyog aranyszínben az eső után, de parazita?", ("Niar", new[] { "Enarc", "Tfiws", "Niar", "Triton" }, "Csillogása csalfa, és elpusztít mindent, amit érint.", "Wert - Termetes egyedek területeinken (Növények)") },
            { "Ki a híres kocsma tulajdonosa, ahol Feiht megfordult?", ("Briktuh", new[] { "Briktuh", "Gordak", "Valera", "Ferth" }, "Ismert, de titkait csak a kiválasztottak ismerik.", "Qwadty - Az Arany Menyét") }
        };

        Profanities = new List<string> { "anyád", "kurva", "gyökér", "baszd", "bazd", "román", "cigány", "buzi", "tetves", "rohadt", "büdös", "szájbatekert", "fasz", "pöcs", "köcsög", "szajha" };

        CommandHandlers = new Dictionary<string[], Action>
        {
            { new[] { "exit", "kilép", "vége" }, ExitGame },
            { new[] { "mondj többet", "bővebben", "tipp", "segít", "help", "hint" }, () => ProvideHint(CurrentQuestion) },
            { new[] { "nem tudom", "feladom" }, () => HandleDontKnow(CurrentQuestionAnswer) },
            { new[] { "debug", "hibakeresés" }, ToggleDebugMode },
            { Profanities.ToArray(), HandleProfanity }
        };

        HelpResponses = new List<string>
        {
            "Segítséget kérsz? Talán gyengébb vagy, mint hittem...",
            "Nem látod az összefüggéseket? Talán túl sötét van körülötted...",
            "Nyomokat akarsz? Figyelj jobban, minden ott van előtted.",
            "A válasz már ott van a fejedben. Kérj, ha mégsem találod.",
            " Nem tudod? Gyengeség. Próbáld újra."
        };

        GreetingResponses = new Dictionary<string, string>
        {
            { "szevasz", "Szevasz, utazó. Az árnyak figyelnek..." },
            { "szia", "Szia. Vajon meddig bírod a sötétben?" },
            { "jó napot", "Jó napot. De vajon tényleg az?" },
            { "helló", "Helló. Lépj be, ha mersz." },
            { "üdv", "Üdvözlet. A sötétség már vár rád." }
        };
        
        System.Random rand = new System.Random();
        var questionEntry = Questions.ElementAt(rand.Next(Questions.Count));
        CurrentQuestion = (questionEntry.Key, questionEntry.Value.CorrectAnswer, questionEntry.Value.AnswerChoices, questionEntry.Value.Hint, questionEntry.Value.Book);
        CurrentQuestionAnswer = questionEntry.Value.CorrectAnswer;
    }

    public void OnEnable()
    {
        DebugMode = false;
        AskQuestion();
    }
    
    public void WriteToTextField(string text)
    {
        string output = $"<color #DDE705>{text}</color>\n";
        textField.text += output == LastOutput ? "" : output;
        LastOutput = output;
    }
    
    // public void StartGame()
    // {
    //     WriteToTextField($"{aiName}: Beléptél a szabadulószobába. Nincs visszaút. A kérdések sorsa a tiéd... Írj 'segíts', 'mondj többet', 'nem tudom', 'debug', 'exit' vagy egy magyar köszönést.");
    //     AskQuestion(false);
    // }

    private void AskQuestion()
    {
        string text = "";
        text += $"{aiName}: {CurrentQuestion.Question}\n";
        //WriteToTextField($"(Könyv: {CurrentQuestion.Book})");
        if (ReadBooks.Contains(CurrentQuestion.Book))
        {
            text += "Lehetőségek:\n";
            foreach (var option in CurrentQuestion.AnswerChoices)
            {
                text += $"- {option}\n";
            }
        }
        else
            text += $"(nyomokért olvasd el a könyveket)\n";
        WriteToTextField(text);
    }

    public void GetAnswer()
    {
        string input = inputField.text.Trim().ToLower();
        textField.text += $"{playerPrompt} {input}\n";
        HandleCommand(input);
    }

    private bool HandleCommand(string input)
    {
        string normalizedInput = RemoveAccents(input);

        foreach (var command in CommandHandlers)
        {
            if (command.Key.Any(cmd => Regex.IsMatch(normalizedInput, CreatePattern(cmd), RegexOptions.IgnoreCase)))
            {
                command.Value.Invoke();
                return true;
            }
        }

        foreach (var greeting in GreetingResponses)
        {
            if (Regex.IsMatch(normalizedInput, CreatePattern(greeting.Key), RegexOptions.IgnoreCase))
            {
                WriteToTextField($"{aiName}: {greeting.Value}");
                return true;
            }
        }

        if (Profanities.Any(p => Regex.IsMatch(normalizedInput, CreatePattern(p), RegexOptions.IgnoreCase))) 
        { 
            HandleProfanity(); 
            return true;
        }

        if (normalizedInput == RemoveAccents(CurrentQuestionAnswer.ToLower()))
        {
            WriteToTextField($"{aiName}: Helyes. De ne hidd, hogy megúszod ennyivel.");
            return true;
        }

        WriteToTextField($"{aiName}: Tévedtél. Egy lépéssel közelebb kerültél a sötétséghez.");
        return false;
    }

    private void ExitGame()
    {
        WriteToTextField($"{aiName}: Megfutamodsz? Legyen hát. De vissza ne térj.");
        Environment.Exit(0);
    }

    private void ProvideHint((string Question, string CorrectAnswer, string[] AnswerChoices, string Hint, string Book) question)
    {
        if (ReadBooks.Contains(question.Book))
            WriteToTextField($"{aiName}: {question.Hint}");
    }

    private void HandleDontKnow(string answer)
    {
        System.Random rand = new System.Random();
        if (DebugMode)
        {
            WriteToTextField($"{aiName}: A válasz: {answer}. Tanuld meg... vagy vesztettél.");
        }
        else
        {
            WriteToTextField($"{aiName}: {HelpResponses[rand.Next(HelpResponses.Count)]}");
        }

        ProvideHint(CurrentQuestion);
    }

    private void HandleProfanity()
    {
        WriteToTextField($"{aiName}: A szavaiddal a sötétséget szítod. Vigyázz, mert a szabadulószoba mesterének türelme véges.");
        GameData.sanity--;
    }

    private void ToggleDebugMode()
    {
        DebugMode = !DebugMode;
        WriteToTextField($"{aiName}: A hibakeresés mód {(DebugMode ? "bekapcsolva" : "kikapcsolva")}. A hideg valóság minden titkát felfedheted.");
    }

    private string RemoveAccents(string input)
    {
        string normalized = input.Normalize(NormalizationForm.FormD);
        char[] chars = normalized
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray();
        return new string(chars).Normalize(NormalizationForm.FormC);
    }

    private string CreatePattern(string input)
    {
        string pattern = Regex.Replace(input, "[éáíóöőúüű]", "[eaiououu]");
        return pattern;
    }
}