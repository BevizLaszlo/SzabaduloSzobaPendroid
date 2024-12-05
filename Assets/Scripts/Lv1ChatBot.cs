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
    [SerializeField] private GameObject doorLock;
    public Dictionary<string, (string CorrectAnswer, string[] AnswerChoices, string Hint, string Book)> Questions { get; set; }
    public Dictionary<string[], Action> CommandHandlers { get; set; }
    public List<string> HelpResponses { get; set; }
    public Dictionary<string, string> GreetingResponses { get; set; }

    public (string Question, string CorrectAnswer, string[] AnswerChoices, string Hint, string Book) CurrentQuestion { get; set; }
    public string CurrentQuestionAnswer { get; set; }
    public bool DebugMode { get; set; }
    public List<string> Profanities { get; set; }
    
    private string LastOutput;

    private int karomkodasCount = 0;
    
    private int NeedToAnswerCount = 3;

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

        Profanities = new List<string>
        {
            "Abberált", "Abnormális", "Abortuszmaradék", "Abuzív", "Ádáz", "Aggasztóan buta", "Agresszív",
            "Ágrólszakadt", "Agyabugyált", "Agyalágyult", "Agyatlan", "Agybajos", "Ágybavizelős", "Agybeteg", "Ágyék",
            "Agyfasz", "Agyhalott", "Agyilag zokni", "Agyonkúrt", "Agyonvert", "Agyrákos", "Agytröszt", "AIDS-es",
            "Akadékoskodó", "Akaratos", "Akasztófavirág", "Alakoskodó", "Alamuszi", "Alantas", "Alattomos", "Alávaló",
            "Aljadék", "Aljas", "Alkalmatlan", "Alkesz", "Alkoholista", "Állat", "Állatkínzó", "Állhatatlan", "Álnok",
            "Alpári", "Álszent", "Alulmaradt", "Alultáplált", "Ámító", "Analfabéta", "Anális", "Anarchista",
            "Annyira barom", "Antipatikus", "Antiszemita", "Antiszociális", "Ánusz", "Ánuszrózsa", "Anyagi kár",
            "Anyagias", "Anyámasszony katonája", "Anyaszomorító", "Apafej", "Apatej", "Ápolatlan", "Áporodott",
            "Aprófaszú", "Aranyeres", "Arcátlan", "Arrogáns", "Ártalmas", "Ártó szándékú", "Árulkodós", "Áruló",
            "Áskálódó", "Aszalék", "Aszexuális", "Aszott", "Átbaszott", "Átejtett", "Átkozott", "Átverhető", "Átvert",
            "Autokrata", "Azt a kurva, de fasz", "Bababöfi", "Bagós", "Bájdorong", "Bájgúnár", "Baktérium", "Balek",
            "Balfácán", "Balfasz", "Balfék", "Balga", "Balhere", "Baljós", "Balkáni", "Balkezes", "Bamba", "Bánat",
            "Bandita", "Bandzsa", "Bántalmazó", "Bántó", "Banya", "Bányarém", "Barátságtalan", "Barbár", "Bárgyú",
            "Barom", "Baromarcú", "Baromfi", "Baszadék", "Basznivaló", "Baszott fasz", "Batári", "Becsavarodott",
            "Becsmérlő", "Becstelen", "Becsületsértő", "Bedrogozott", "Beégetett", "Befingott", "Befosi", "Begolyózott",
            "Begyöpösödött", "Behemót", "Bekasztlizott", "Beképzelt", "Békétlen", "Bekómált", "Bél", "Bélböfi",
            "Bélféreg", "Bélférges", "Bélsár", "Bélszél", "Béltartalom", "Béna", "Benga", "Bennszülött", "Berosált",
            "Berúgott", "Besavanyodott", "Bestia", "Besúgó", "Beszari", "Beszívott", "Beteg", "Betyár", "Bibircsókos",
            "Bigott", "Birka", "Biszexuális", "Bitang", "Bitóravaló", "Bódult", "Bogaras", "Bohóc", "Boldogtalan",
            "Bolhás", "Bolond", "Bolondos", "Bolsevik", "Bomlott elméjű", "Borús", "Borzadály", "Borzalmas", "Boszorka",
            "Boszorkány", "Bosztohó", "Botor", "Botrányos", "Bóvli", "Bődületes", "Böfögős", "Böhöm", "Bőrfurulya",
            "Bőrkiütéses", "Börtöntöltelék", "Börtönviselt", "Böszme", "Bráner", "Bré", "Broki", "Brunya", "Brunyálás",
            "Brutális", "Búbánat", "Budi", "Buggyant", "Bugris", "Bugyuta", "Bujdosó", "Bujkáló", "Bukott",
            "Bumburnyák", "Bumfordi", "Bunkó", "Bús", "Búskomor", "Buta", "Butácska", "Butus", "Butuska",
            "Búvalbaszott", "Búza", "Búzacsíra", "Buzeráns", "Buzernyák", "Buzgómócsing", "Buzi", "Büdös", "Büdösszájú",
            "Bűnben járó", "Bűnelkövető", "Bűnös", "Bűnöző", "Bütykös", "Bűzölgő", "Bűzös", "Cafat", "Cafka", "Céda",
            "Céltalan", "Cicafiú", "Cici", "Cigány", "Ciki", "Cingár", "Cinikus", "Csacsi", "Csacska", "Csacsogó",
            "Csalafinta", "Csalárd", "Csálé", "Csalfa", "Csaló", "Csámcsogó", "Csámpás", "Csapnivaló", "Csapodár",
            "Csapzott", "Csavargó", "Csecsszopó", "Cselszövő", "Csempész", "Csenevész", "Cseszett", "Csetlő-botló",
            "Csibész", "Csicska", "Csicskagyász", "Csimbókos", "Csinovnyik", "Csintalan", "Csínytevő", "Csipás",
            "Csíra", "Csirkefiú", "Csirkefogó", "Csiszolatlan", "Csitri", "Csócsált", "Csoffadt", "Csonthülye", "Csóró",
            "Csoroszlya", "Csótány", "Csotrogány", "Csöcs", "Csőcselék", "Csőd", "Csökevény", "Csökönyös", "Csökött",
            "Csőlakó", "Csőlátású", "Csöpögős", "Csöves", "Csúf", "Csuhás", "Csula", "Csumpi", "Csúnya", "Csupaszfaszú",
            "Csüngőcsöcsű", "Csürhe", "Cucli", "Cudar", "Cukrosbácsi", "Cunci", "Cuppogós", "Dadogós", "Dagadt",
            "Daganatos", "Dagi", "Dákó", "Debella", "Debil", "Dedós", "Degenerált", "Demagóg", "Depressziós", "Deviáns",
            "Didi", "Dildó", "Dilettáns", "Dilibogyó", "Dilinyós", "Dilis", "Dilló", "Diló", "Dinka", "Dinnye",
            "Díszbuzi", "Disznó", "Divatmajom", "Dög", "Dögevő", "Dölyfös", "Dőre", "Drekk", "Drogfüggő", "Drogos",
            "Droid", "Duda", "Dudaorrú", "Dughatatlan", "Dundi", "Durung", "Durva", "Duzzogó", "Dühös", "Dünnyögő",
            "Dzsipszi", "Dzsudva", "Dzsuva", "Ebadta", "Ebfasz", "Egészen barom", "Egoista", "Égő", "Egyszerű",
            "Együgyű", "Ehetetlen", "Elagyabugyált", "Elalélt", "Elásnivaló", "Elátkozott", "Elavult", "Elázott",
            "Elbaltázott", "Elbaszott", "Elbizakodott", "Elburjánzott", "Elcsépelt", "Elégedettlen", "Életellenes",
            "Életképtelen", "Életveszélyes", "Eleve hülye", "Elfajult", "Elfenekelt", "Elfogult", "Elfuserált",
            "Elgyepált", "Elhervadt", "Élhetetlen", "Elhibázott", "Elhidegült", "Elhízott", "Elintézett", "Elítélt",
            "Elkallódott", "Elkámpicsorodott", "Elkaszált", "Elkenődött", "Elképesztő", "Elképzelhetetlen",
            "Elkeserítő", "Elkorcsosult", "Elkótyavetyélt", "Elkúrt", "Ellenséges", "Ellenszenves", "Elmaradott",
            "Elmebajos", "Elmebeteg", "Elmeháborodott", "Elmeroggyant", "Elmezavarodott", "Elmezavaros", "Elnáspángolt",
            "Elnyomott", "Előítéletes", "Élősködő", "Elrákosodott", "Elrettentő", "Elrontott", "Elrugaszkodott",
            "Elsorvadt", "Eltaposott", "Eltorzított", "Eltűrhetetlen", "Elutasított", "Elvakult", "Elvetélt",
            "Elvetemült", "Élvezhetetlen", "Elviselhetetlen", "Elvtelen", "Elzüllött", "Embertelen", "Emésztőgödör",
            "Emlékezet-kihagyásos", "Emlő", "Engedetlen", "Enyveskezű", "Epeköves", "Epilepsziás", "Érdektelen",
            "Érelmeszesedéses", "Éretlen", "Eretnek", "Érfelvágós", "Ergya", "Erkölcstelen", "Ernyedtfaszú",
            "Erőszakos", "Erőtlen", "Értéktelen", "Értelmetlen", "Értelmezhetetlen", "Értelmi fogyatékos", "Értetlen",
            "Érthetetlen", "Érvénytelen", "Érzéketlen", "Esetlen", "Eszelős", "Eszement", "Eszetlen", "Eszeveszett",
            "Észkombájn", "Eszméletlen", "Észosztó", "Észszerűtlen", "Esztelen", "Ételmaradék", "Ételmérgezéses",
            "Exhibicionista", "Extrahülye", "Fafej", "Fafejű", "Fajankó", "Fajgyűlölő", "Fajtalankodó", "Fakír",
            "Faksznis", "Falfirkász", "Falhoz baszott", "Falrahányt", "Falusi", "Fancsali", "Fanyar", "Fapados",
            "Fapina", "Far", "Faragatlan", "Fárasztó", "Farok", "Farokcibáló", "Farpofa", "Fasiszta", "Fasz", "Faszarc",
            "Faszcibáló", "Faszimádó", "Faszkalap", "Faszkarika", "Faszkedvelő", "Faszkópé", "Faszmajszoló",
            "Faszogány", "Faszpörgettyű", "Faszrajongó", "Faszszagú", "Faszszopó", "Faszszőr", "Fasztalan",
            "Fasztalanított", "Fasztarisznya", "Faszváladék", "Faszverő", "Fatökű", "Fattyú", "Fecni", "Fegyenc",
            "Fejetlen", "Fejletlen", "Feka", "Fekália", "Féktelen", "Felaprított", "Félbeszakadt", "Felcsinált",
            "Feldúlt", "Félelemkeltő", "Félelmetes", "Felelőtlen", "Felesleges", "Féleszű", "Felfuvalkodott",
            "Felhergelt", "Félkegyelmű", "Félkész", "Felkészületlen", "Fellengzős", "Félnótás", "Felnyársalt",
            "Felöklendezett", "Félős", "Felpofozott", "Félrebaszott", "Félrecsinált", "Félreérthető", "Félresikerült",
            "Félrevezető", "Félszeg", "Felszínes", "Felületes", "Fenegyerek", "Fenék", "Fenevad", "Fennhéjázó",
            "Ferdefaszú", "Ferdehajlamú", "Ferdeszemű", "Féreg", "Férges", "Fergetegesen fasz", "Feslett", "Feszült",
            "Fetisiszta", "Ficsúr", "Fidesz-bérenc", "Fideszes", "Fika", "Fing", "Fingfejű", "Fingfelhő", "Fingszagú",
            "Finnyás", "Fintorgó", "Fittyethányó", "Fityma", "Fitymafaggyú", "Fitymaszűkület", "Flegma", "Flepnis",
            "Flótás", "Flúgos", "Fogatlan", "Fogyatékkal élő", "Fogyatékos", "Fonnyadt", "Fonnyadt csöcsű", "Forrófejű",
            "Fos", "Foscunami", "Fosfej", "Foshalom", "Foshenger", "Foslavor", "Fosós", "Fosömleny", "Fosparádé",
            "Fospermet", "Fospumpa", "Fostalicska", "Fostartály", "Fostos", "Fostölcsér", "Fosvihar", "Fosvödör",
            "Fosztogató", "Foszuhany", "Foszuhatag", "Fölényeskedő", "Förmedvény", "Förtelem", "Förtelmes", "Fösvény",
            "Fránya", "Frász", "Fráter", "Frigid", "Fruska", "Frusztrált", "Fukar", "Furkó", "Füllentő", "Füllentős",
            "Fülöncsípett", "Fütyi", "Fütykös", "Füves", "Gagyi", "Galád", "Galandféreg", "Ganajtúró", "Gané",
            "Ganédomb", "Gány", "Gátlásos", "Gátlástalan", "Gatter", "Gatya", "Gáz", "Gaz", "Gazember", "Gazfickó",
            "Gázos", "Gazsi", "Gebe", "Geca", "Gecc", "Geci", "Gecinyelő", "Genetikai baleset", "Genetikai hulladék",
            "Genetikai korcs", "Gengszter", "Génhibás", "Genny", "Gennyes", "Gennyes csöcsű", "Gennyesfaszú",
            "Gennyesszájú", "Gennyező", "Gennygóc", "Gennyszopó", "Genya", "Genyac", "Genyó", "Gerincferdüléses",
            "Gerincre vágott", "Gerinctelen", "Giccses", "Giliszta", "Girbe-gurba", "Girhes", "Girnyó", "Gittes",
            "Gizda", "Gnóm", "Góc", "Gólyafos", "Golyhó", "Golyós", "Gombás", "Gombás csöcsű", "Gond", "Gondterhelt",
            "Gonosz", "Gonosztevő", "Gorilla", "Goromba", "Gőgös", "Görbefaszú", "Görcs", "Görcsös", "Görény", "Göthös",
            "Grimaszolós", "Groteszk", "Gúnyos", "Gusztustalan", "Gügye", "Gülüszemű", "Gyagya", "Gyagyás",
            "Gyakorlatlan", "Gyalázat", "Gyalázatos", "Gyámolatlan", "Gyámoltalan", "Gyanús", "Gyanúsított",
            "Gyári hibás", "Gyarló", "Gyártási hibás", "Gyász", "Gyászhuszár", "Gyászkeret", "Gyászos", "Gyatra",
            "Gyáva", "Gyenge", "Gyengeelméjű", "Gyengélkedő", "Gyepált", "Gyépés", "Gyér", "Gyerekes", "Gyerekgyilkos",
            "Gyerekrabló", "Gyík", "Gyíkfing", "Gyilkos", "Gyógyegér", "Gyógyíthatatlan", "Gyógykezeléses",
            "Gyógykezelt", "Gyogyós", "Gyógyszeres", "Gyomorforgató", "Gyomortartalom", "Gyökér", "Gyönge", "Gyöp",
            "Gyötrelmes", "Gyűlölnivaló", "Gyűröttképű", "Habókos", "Háborodott", "Habzószájú", "Hájfej", "Hajléktalan",
            "Hajmeresztő", "Hájpacni", "Hajthatatlan", "Halál", "Halálraítélt", "Halandzsázó", "Hálátlan", "Halott",
            "Hamis", "Hámlóképű", "Hancúrléc", "Hangyás", "Hantás", "Hányadék", "Hanyag", "Hányás", "Hányás ízű",
            "Hányás szagú", "Hanyatló", "Hányinger", "Hányingerkeltő", "Harácsoló", "Haragos", "Haramia", "Harapós",
            "Hárpia", "Hasmenés", "Hasznavehetetlen", "Haszonleső", "Haszontalan", "Hatalmas nagy fasz", "Hatalmaskodó",
            "Határozatlan", "Hatástalan", "Hátbabaszott", "Hátborzongató", "Hatökör", "Hátsó fertály", "Hazaáruló",
            "Hazátlan", "Hazavágott", "Házisárkány", "Hazudozó", "Hazug", "Hebehurgya", "Helybenhagyott", "Helytelen",
            "Hencegő", "Here", "Herélt", "Hererákos", "Heretapogató", "Herétlen", "Hergelt", "Hermafrodita", "Hernyó",
            "Hernyótalpas", "Herpeszes", "Hervadt", "Hétszentséges", "Hetyke", "Hiábavaló", "Hibás", "Hibbant",
            "Hidegre tett", "Hidegvérű", "Híg fos", "Hígagyú", "Hígvelejű", "Hihetetlenül fasz", "Hikomat",
            "Hímivarsejt", "Hímnőstény", "Himpellér", "Hímringyó", "Hímsoviniszta", "Hímtag", "Hímvessző", "Hippi",
            "Hírhedt", "Hiszékeny", "Hisztis", "Hitetlen", "Hitler-imádó", "Hitlerista", "Hitvány", "Hiú",
            "HIV-pozitív", "Hóbelebanc", "Hóbortos", "Hóhér", "Hokerós", "Holdkóros", "Hólyag", "Homár", "Hombár",
            "Homofób", "Homokos", "Homoszexuális", "Hónaljkutya", "Hónaljszagú", "Hontalan", "Horkolós", "Hormonkezelt",
            "Hormonzavaros", "Hörcsögképű", "Hörcsögpofa", "Hú, de barom", "Húgy", "Húgyagyú", "Húgymeleg", "Hugyos",
            "Húgytócsa", "Húgyúti fertőzés", "Huligán", "Hulla", "Hulladék", "Hullagyalázó", "Hullarabló",
            "Hűbelebalázs", "Hüle", "Hülye", "Hülyécske", "Hülyegyerek", "Hűtlen", "Hüvely", "Hüvelygombás",
            "Hűvösre tett", "Idegbajos", "Idegbeteg", "Idegen", "Idegengyűlölő", "Ideges", "Idegesítő", "Idejétmúlt",
            "Idétlen", "Idióta", "Idomíthatatlan", "Idomtalan", "Igenállat", "Ignorálható", "Ijesztő",
            "Ijesztően hülye", "Illegális", "Illemtelen", "Illetlen", "Imbecilis", "Impotens", "Incifinci",
            "Indiszponált", "Indokolatlanul fasz", "Indulatos", "Infantilis", "Ingyenélő", "Injekciózott",
            "Inkubátor-szökevény", "Intoleráns", "Ipari baleset", "Ipari hulladék", "IQ bajnok", "IQ hiányos",
            "Irdatlan", "Irgalmatlan", "Irgalmatlanul fasz", "Irigy", "Irracionális", "Irreális", "Irritáló",
            "Istenátka", "Istentelen", "Istenverte", "Iszákos", "Ittas", "Ivadék", "Ivartalanított", "Izé",
            "Izgő-mozgó", "Ízlésficamos", "Ízléstelen", "Izomagy", "Izomagyú", "Izzadós", "Izzadtságszagú", "Izzó",
            "Jaj, de barom", "Jajveszékelő", "Járókeretes", "Javíthatatlan", "Javítóintézetes", "Jelentéktelen",
            "Jellegtelen", "Jellemtelen", "Jöttment", "Kába", "Kábítószeres", "Kábult", "Kaján", "Kajla", "Kaka",
            "Kákabélű", "Kakamaradék", "Kakamatyi", "Kaki", "Kakibohóc", "Kaksi", "Kalácsképű", "Kalafüttyös",
            "Kamugép", "Kamuzós", "Kanca", "Kancigány", "Kancsal", "Kandisznó", "Kangörcsös", "Kannibál", "Kanos",
            "Kapitalista", "Kapzsi", "Karattyolós", "Karóba húzott", "Károkozó", "Káromkodós", "Káros",
            "Káros mellékhatás", "Kárörvendő", "Kártékony", "Kártevő", "Katasztrófa", "Kecskebaszó", "Kegyetlen",
            "Kehes", "Kéjrúd", "Kelekótya", "Kellemetlen", "Kellemetlenkedő", "Kelletlen", "Keményfejű", "Kényelmetlen",
            "Kenyérpusztító", "Kényes", "Kényszeres", "Kényszeríthető", "Kényszerképzetes", "Kepesztős", "Képtelen",
            "Kéregető", "Kerékkötő", "Kéretlen", "Kerge", "Kérkedő", "Kérlelhetetlen", "Keserves", "Keshedt",
            "Kétbalkezes", "Kétballábas", "Kétes", "Kétfaszú", "Kétséges", "Kétszer agyonbaszott", "Kétszínű",
            "Kettékúrt", "Kettyós", "Kevély", "Keverék", "Kevés", "Kezdő", "Ki-bebaszott", "Kiakasztott",
            "Kialakítatlan", "Kibaszott", "Kibírhatatlan", "Kibuherált", "Kibuzerált", "Kicsapongó", "Kicsinált",
            "Kicsinyes", "Kielégíthetetlen", "Kiemelkedően barom", "Kifacsart", "Kifingott", "Kifogásolható",
            "Kigolyózott", "Kiherélt", "Kikakkantott", "Kiképzetlen", "Kikészült", "Kiközösített", "Kikúrt",
            "Kimagaslóan fasz", "Kimerítő", "Kínkeserves", "Kínos", "Kínoznivaló", "Kipurcant", "Kirekesztő",
            "Kirettyintett", "Kirívó", "Kiröhögnivaló", "Kis szaros", "Kisemmizett", "Kísérleti patkány", "Kiskancsó",
            "Kispadra tett", "Kispályás", "Kispatak", "Kissebbségi", "Kiszolgáltatott", "Kitagadott", "Kitett szűrű",
            "Kitüremkedett", "Kivagyi", "Kivénhedt", "Kivert", "Kleptomániás", "Klotyószagú", "Koca", "Kockafejű",
            "Kocsmatöltelék", "Kókadt", "Kókány", "Kókler", "Koldus", "Kolerás", "Kómás", "Komisz", "Kommentálhatatlan",
            "Kommunista", "Komolytalan", "Komor", "Konok", "Kontár", "Kopasz", "Kópé", "Kopott", "Koppintott",
            "Korai magömlés", "Korcs", "Korhadt", "Korhatáros", "Korlátolt", "Kórokozó", "Kóros", "Korrigálhatatlan",
            "Korrodált", "Korrupt", "Kosz", "Koszfészek", "Koszlidérc", "Koszos", "Kotnyeles", "Kotonszökevény",
            "Kotorék", "Kozmikus szemét", "Köcsög", "Köcsögváza", "Könnyelmű", "Könyörtelen", "Köpcös", "Köpet",
            "Köpködős", "Köpönyegforgató", "Kötekedő", "Kövér", "Középszar", "Középszerű", "Közhelyes", "Közömbös",
            "Közönséges", "Közönyös", "Közröhely tárgya", "Krákogós", "Krepálós", "Kretén", "Kripli", "Kriptaszökevény",
            "Kritikán aluli", "Krumplifejű", "Kufircolós", "Kuka", "Kukabúvár", "Kukac", "Kukelló", "Kuki", "Kula",
            "Kultúrálatlan", "Kuncsorgó", "Kunyerálós", "Kupán vágott", "Kupori", "Kurafi", "Kúrhatatlan", "Kurjantós",
            "Kúrós", "Kurtafarkú", "Kurva", "Kurvapecér", "Kutya", "Kutyagumi", "Kutyakaki", "Kutyapiszok", "Kutyaszar",
            "Különcködő", "Labilis", "Lajhár", "Láma", "Langyos", "Lankadék", "Lankadt", "Lankadtfaszú", "Lapátra tett",
            "Lapostetű", "Lárva", "Lator", "Latyak", "Latymatag", "Lé", "Leamortizált", "Leaszalt", "Lebaszirgált",
            "Lebaszott", "Lecseszett", "Lecsúszott", "Ledér", "Leégett", "Leélt", "Lefitymáló", "Léha", "Léhűtő",
            "Leírhatatlanul fasz", "Lejáratott", "Lejárt szavatosságú", "Lejmolós", "Lekezelő", "Lekicsinylő", "Lekvár",
            "Lelenc", "Lelketlen", "Lelombozódott", "Lelőtt", "Lemenstruált", "Lenéző", "Lenyaltképű", "Leokádott",
            "Lepedék", "Lepkefing", "Leprafészek", "Leprás", "Lepusztult", "Lerobbant", "Leszart", "Leszbikus",
            "Leszopott", "Letargiába esett", "Letargikus", "Letartóztatott", "Leterhelt", "Levakarhatatlan",
            "Levéltetű", "Levert", "Levitézlett", "Lezüllött", "Liba", "Libafos", "Likvidált", "Link", "Linkóci",
            "Lóbré", "Lócitrom", "Lódító", "Lódög", "Lófasz", "Lógócsöcsű", "Lóhugy", "Lókötő", "Lólőcs", "Lom",
            "Lombikbébi", "Lomha", "Lomtár", "Lópikula", "Lotyó", "Lőcs", "Lőcsgéza", "Lökött", "Lőtt seb", "Löttyedt",
            "Lucskos", "Lucsok", "Luk", "Lusta", "Luvnya", "Lúzer", "Lüke", "Lüttyő", "Lütyő", "Lyuk", "Lyukasbelű",
            "Lyukasfaszú", "Lyukaskezű", "Lyukát vakaró", "Macerás", "Macskafos", "Macskahányadék", "Mafla", "Majom",
            "Majrés", "Majrézós", "Makacs", "Makk", "Malac", "Málé", "Málészájú", "Mamlasz", "Mamutseggű", "Mániákus",
            "Mániás", "Manipulált", "Mankós", "Maradi", "Marcona", "Marha", "Másokat lenéző", "Maszlag", "Maszturbagép",
            "Maszturbálós", "Matatós", "Mazochista", "Mazsola", "Mega-barom", "Megalapozatlan", "Megalázkodó",
            "Megalkuvó", "Megápolt", "Megátalkodott", "Megbaszott", "Megbízhatatlan", "Megbukott", "Megdöbbentő",
            "Megdönthető", "Megdugott", "Megemészthetetlen", "Megerőszakolt", "Megfélemlített", "Megfingatott",
            "Meggondolatlan", "Meghökkentő", "Meghunyászkodó", "Meghuzatott", "Megjátszós", "Megkatéterezett",
            "Megkattant", "Megkergült", "Megkeseredett", "Megkettyintett", "Megkúrt", "Megmart", "Megoldhatatlan",
            "Megrakott", "Megrontott", "Megskalpolt", "Megszakított", "Megszokhatatlan", "Megszomorodott",
            "Megszopatott", "Megtébolyodott", "Megtébolyult", "Megtépázott", "Megtévesztő", "Megveszekedett", "Megvető",
            "Megzavarodott", "Megzuhant", "Mélabús", "Melák", "Meleg", "Mell", "Mellbimbó", "Mellékhatás", "Mellékhere",
            "Melléktermék", "Mellérakott", "Mellrákos", "Menedékkérő", "Menekült", "Menesztett", "Mentálisan sérült",
            "Menthetetlen", "Méreg", "Mérgezett", "Mérgező", "Mérgező hulladék", "Mérhetetlenül ostoba",
            "Mérsékelt képességű", "Mérsékelten okos", "Méteres kékeres", "Migráns", "Migrénes", "Mihaszna",
            "Mitugrász", "Mocsadék", "Mócsing", "Mócsingos", "Mocskos", "Mocskosszájú", "Mocsok", "Moderálatlan",
            "Módfelett barom", "Modortalan", "Mogorva", "Mohácsi vész", "Mohó", "Mojfing", "Mólés", "Molesztált",
            "Molett", "Mony", "Morbid", "Morcos", "Moslék", "Mostoha", "Mószerolós", "Motyogós", "Mucsai", "Muff",
            "Mugli", "Mulya", "Munkamániás", "Munkanélküli", "Mutáns", "Mutogatós", "Műfaszú", "Műmellű", "Műtéti heg",
            "Műtéti seb", "Műtöttképű", "Műveletlen", "Náci", "Nagyarcú", "Nagyképű", "Nagyon barom", "Nagyothalló",
            "Nagyotmondó", "Nagypofájú", "Nagyravágyó", "Nagyszájú", "Nagyzoló", "Naplopó", "Napszúrásos",
            "Nárcisztikus", "Narkós", "Náspángolt", "Negédes", "Négyszemű", "Nehéz eset", "Nehéz felfogású",
            "Nehezen emészthető", "Nehézfejű", "Neheztelő", "Nekrofil", "Nélkülözhető", "Nem az igazi",
            "Nem beszámítható", "Nem jó", "Nem komplett", "Nem normális", "Nem százas", "Nem várt hiba", "Némber",
            "Nemi szerv", "Népirtó", "Neurózisos", "Neveletlen", "Nevelőintézetes", "Nevetség tárgya", "Nevetséges",
            "Nézeteltérés", "Nézhetetlen", "Nigger", "Nikotinpatkány", "Nímand", "Nimfomániás", "Nívótlan", "Nokedli",
            "Nonszensz", "Normálatlan", "Növény", "Nudista", "Nukleáris hulladék", "Nulla", "Nuna", "Nunci", "Nuni",
            "Nyafogó", "Nyakatekert", "Nyakoncsípett", "Nyakonöntött", "Nyál", "Nyáladzó", "Nyalakodó", "Nyálas",
            "Nyálcsorgató", "Nyalizó", "Nyalizós", "Nyalóka", "Nyalonc", "Nyalós", "Nyáltenger", "Nyámnyila",
            "Nyamvadt", "Nyanya", "Nyápic", "Nyasgem", "Nyavaja", "Nyavajás", "Nyavajgó", "Nyegle", "Nyekergő",
            "Nyelestojás", "Nyers", "Nyeszle", "Nyeszlett", "Nyikhaj", "Nyiszlett", "Nyivákolós", "Nyomorék",
            "Nyomoronc", "Nyomorult", "Nyomott", "Nyugtatózott", "Nyúl", "Nyúlbéla", "Nyúlszar", "Nyuszi",
            "Nyuvasztott", "Nyünyüke", "Nyüsszögő", "Nyüsszögős", "Nyüves", "Nyüzge", "Nyüzüge", "Óbégatós", "Obszcén",
            "Ócsárló", "Ócska", "Ocsmány", "Ódivatú", "Okádék", "Okoskodó", "Oktondi", "Oltári nagy fasz", "Ondó",
            "Ondónyelő", "Opott", "Optikás", "Orángután", "Orbitálisan fasz", "Ordenáré", "Ormótlan", "Oroszlánszagú",
            "Orrát fennhordó", "Orrát mindenbe beleütő", "Orrbapúzott", "Orrváladék", "Ortó", "Ortopéd", "Orvosi eset",
            "Orvosi műhiba", "Ósdi", "Ostoba", "Otromba", "Otthontalan", "Ótvar", "Ótvaros", "Óvodás",
            "Óvszerszökevény", "Öklendező", "Ökör", "Ökörállat", "Öncsonkító", "Önelégült", "Önfejű", "Önfényező",
            "Önhitt", "Önimádó", "Önmarcangoló", "Önpusztító", "Öntelt", "Önző", "Ördög", "Ördögfajzat", "Ördögfióka",
            "Ördögi", "Örökvesztes", "Örömlány", "Örömtelen", "Őrült", "Összeaszalódott", "Összeaszott", "Összebaszott",
            "Összetöpörödött", "Ösztönösen barom", "Ötlettelen", "Paccer", "Pajzán", "Pali", "Palimadár", "Palira vett",
            "Pampogó", "Panaszkodó", "Pancser", "Paprikajancsi", "Papucs", "Paranormális", "Paraszt", "Parazita",
            "Parázós", "Paréj", "Pártkatona", "Patkány", "Pattanásos", "Peches", "Pedál", "Pedofil", "Példátlanul fasz",
            "Penészes", "Penetráns", "Pénisz", "Pénisz-előváladék", "Péniszváladék", "Péppé zúzott", "Pernahajder",
            "Perszóna", "Perverz", "Pete", "Petyhüdt", "Piás", "Picsa", "Picsafej", "Picsánrúgott", "Picsányi",
            "Picskó", "Picsogó", "Picsusz", "Pihentagyú", "Pimasz", "Pina", "Pinanyaló", "Pinna", "Piperkőc", "Piponya",
            "Piromániás", "Pisaszagú", "Pisi", "Pisis", "Piskóta", "Piszkos", "Piszlicsáré", "Piszok", "Pite", "Piti",
            "Pitiáner", "Pityókás", "Pocsék", "Pofátlan", "Pofázó", "Pojáca", "Pokolfajzat", "Pokoli", "Pokolravaló",
            "Pólyás", "Pomádé", "Pondró", "Pongyola", "Popó", "Popsi", "Populista", "Porbafingó", "Pórias",
            "Pornómániás", "Poshadt", "Pótolható", "Potom", "Pózer", "Pozőr", "Pöce", "Pöcegödör", "Pöcs", "Pöcsfej",
            "Pöffeszkedő", "Pökhendi", "Pöszmét", "Pribék", "Primitív", "Proli", "Prontó", "Pronyó", "Propagandista",
            "Prosti", "Prostituált", "Prosztó", "Protkós", "Protty", "Prűd", "Prütykölős", "Pszichiátriai eset",
            "Pszichológiai eset", "Pszichopata", "Pú", "Pucér", "Pucolatlan", "Pucsító", "Puding", "Pudva", "Pudvás",
            "Pudváslikú", "Puffadt", "Pufi", "Puhafaszú", "Puhány", "Puhapöcsű", "Puki", "Pukizós", "Punci",
            "Puncipöcögtető", "Punna", "Pupák", "Pusztulat", "Putri", "Püspökfalat", "Rabló", "Rabszolga", "Ragyás",
            "Rákkeltő hatású", "Rakoncátlan", "Rákos", "Rakott", "Ramaty", "Ráncos", "Raplis", "Rasszista", "Ratyi",
            "Recskázó", "Redva", "Rekedt", "Rémálom", "Reménytelen", "Reményvesztett", "Rémes", "Rémisztő", "Rendbontó",
            "Rendellenes", "Rendetlen", "Rendkívüli", "Rendszertelen", "Renitens", "Renyhe", "Repedtsarkú", "Rest",
            "Részeg", "Részeges", "Retardált", "Retek", "Retkes", "Rettegett", "Rettenet", "Rettenetes", "Retty",
            "Retyó", "Revdás", "Reves", "Ribanc", "Rideg", "Rigolyás", "Rihe", "Riherongy", "Rihonya", "Rimánkodó",
            "Ringyó", "Rinyalé", "Rinyálós", "Ripacs", "Ripők", "Ritka hülye", "Rivalizáló", "Roggyant", "Rohadék",
            "Rohadt", "Rohadvány", "Róka", "Rókázás", "Rokkant", "Rom", "Roma", "Román", "Rombadöntött", "Romhalmaz",
            "Romlott", "Romlott erkölcsű", "Roncs", "Ronda", "Rondaság", "Rongyos", "Rongyrázó", "Roppant kellemetlen",
            "Roskadt", "Rosseb", "Rossz", "Rossz kedvű", "Rossz leheletű", "Rosszakaró", "Rosszalló", "Rosszaság",
            "Rosszban sántikáló", "Rosszcsont", "Rosszindulatú", "Rosszmájú", "Rosszul eső", "Rosszullét", "Rothadt",
            "Rotty", "Rozoga", "Rozsdásagyú", "Rozzant", "Röfi", "Rőfös fasz", "Röhelyes", "Rugalmatlan", "Rusnya",
            "Rusnyaság", "Rút", "Rücskös", "Rühes", "Rüszök", "Rüszü", "Saját nemével kefélő", "Sakál", "Sanda",
            "Sánta", "Sanyargatott", "Sanyarú", "Sápadt", "Sápadtarcú", "Sarlatán", "Sáros", "Sárvérű", "Sátánfajzat",
            "Sátánista", "Satnya", "Savanyúképű", "Sebhelyes", "Segg", "Segg-lé", "Seggarc", "Seggdugó", "Seggfej",
            "Seggkolbász", "Segglyuk", "Seggnyaló", "Seggváladék", "Sehonnai", "Sejhaj", "Sekélyes", "Selejt",
            "Selejtes", "Selyemfiú", "Semmi", "Semmibevett", "Semmirekellő", "Senki", "Senkiházi", "Sertés", "Sértő",
            "Sértődékeny", "Sérült", "Setesuta", "Settenkedő", "Silány", "Simlis", "Sintér", "Siralmas", "Siralom",
            "Siránkozó", "Sírógép", "Sírós", "Sittes", "Sivár", "Skalpolt", "Skizofrén", "Skót", "Slejm", "Smafu",
            "Smucig", "Snassz", "Sóher", "Sokkolóan barom", "Sokkos", "Sopánkodó", "Sorjázatlan", "Sorozatgyilkos",
            "Sorstalan", "Sorvadt", "Sóvárgó", "Söpredék", "Sötét", "Sperma", "Spermaputtony", "Spermium", "Spicces",
            "Spicli", "Spiné", "Stigmatizált", "Stikkes", "Stiklis", "Stréber", "Strici", "Sudri", "Sugárfertőzött",
            "Suhanc", "Súlyos", "Sumák", "Sunyi", "Surmó", "Suta", "Suttyó", "Sutyerák", "Sügér", "Sükebóka", "Süket",
            "Sületlen", "Süsü", "Svindler", "Szadista", "Szagos", "Szájbatekert", "Szajha", "Szálkafaszú",
            "Szalmonellás", "Szamár", "Szánalmas", "Szánalom", "Szar", "Szarcsimbók", "Szarcunami", "Szarevő",
            "Szarfaszú", "Szarfazék", "Szarfelvágott", "Szargép", "Szarházi", "Szarjankó", "Szarkupac", "Szaros",
            "Szarosvalagú", "Szarömleny", "Szarrágó", "Szarrakás", "Szarszagú", "Szarszájú", "Szarszippantó",
            "Szartragacs", "Szarzsák", "Szarzuhatag", "Szatír", "Szavahihetetlen", "Szédült", "Szegény",
            "Szégyencsicska", "Szégyenletes", "Szégyentelen", "Szégyenteljes", "Széklet", "Székrekedés",
            "Székrekedéses", "Szélcsapó", "Szeleburdi", "Szélhámos", "Széllelbélelt", "Szellemi fogyatékos",
            "Szellemileg visszamaradott", "Szellentés", "Szellentős", "Széltoló", "Szélütéses", "Szemérmetlen",
            "Szemét", "Szemétdomb", "Szemétkupac", "Szemétláda", "Szemfényvesztő", "Szemilis", "Szemölcsös",
            "Szemtelen", "Szenny", "Szennyes", "Szentimentális", "Szenvtelen", "Szeplős", "Szépséghibás",
            "Szerencsétlen", "Szertelen", "Szervilis", "Szeszélyes", "Szeszkazán", "Szétszórt", "Szétvert",
            "Szétzilált", "Szexmániás", "Szifiliszes", "Szipirtyó", "Szittyós", "Szívás", "Szívbajos", "Szivózós",
            "Szívtipró", "Szmötyis", "Szófosó", "Szokatlanul fasz", "Szoknyapecér", "Szolgalelkű", "Szomorú",
            "Szopadék", "Szopógép", "Szopógörcs", "Szopórém", "Szopós", "Szórakozott", "Szószátyár", "Szottyadt",
            "Szökött", "Szörnyeteg", "Szörnyszülött", "Szörnyű", "Szőrösszívű", "Szőrszálhasogató", "Sztálinista",
            "Szuka", "Szuperhülye", "Szutykos", "Szutyok", "Szűk látókörű", "Szűklyukú", "Szűkmarkú",
            "Szűzhártya-repedéses", "Szűzhártya-repesztő", "Szűzkurva", "Tacskó", "Tahó", "Tajparaszt",
            "Takarékra tett", "Taknyos", "Takony", "Takonyevő", "Takonypóc", "Talpnyaló", "Tanulatlan", "Tanyasi",
            "Tapasztalatlan", "Taperálós", "Tapintatlan", "Tapír", "Tapizós", "Tapló", "Tápos", "Tápszer",
            "Tarkónbaszott", "Tata", "TBC-s", "Tébolyodott", "Tébolyult", "Tehén", "Tehetetlen", "Tehetségtelen",
            "Tejbetök", "Telhetetlen", "Telibekúrt", "Telibevert", "Tenyérbemászó", "Tépett", "Természetellenes",
            "Természetre káros", "Testképzavaros", "Teszetosza", "Tesznye", "Tétova", "Tetű", "Tetves", "Tevetrágya",
            "Tikkadt", "Tinó", "Tintás", "Tirpák", "Tiszteletlen", "Tisztességtelen", "Tohonya", "Tolerálhatatlan",
            "Tolószékes", "Tolvaj", "Tompa", "Topa", "Toprongyos", "Torkos", "Torz", "Torzszülött", "Toszatlan",
            "Toszott", "Totál kész", "Totálisan hülye", "Totojázó", "Tök", "Tökéletlen", "Töketlen", "Tökfej",
            "Tökfilkó", "Tökkelütött", "Tökmag", "Tömeggyilkos", "Tönkrement", "Töppedt", "Töpszli", "Törpefaszú",
            "Tragacs", "Trágár", "Trágya", "Trágyalé", "Trampli", "Transzfób", "Transzvesztita", "Tranzszexuális",
            "Tré", "Trehány", "Tróger", "Trombózisos", "Tropa", "Trotty", "Trottyos", "Trutyi", "Trutyis", "Trutymó",
            "Tudathasadásos", "Tudatlan", "Tufa", "Túladagolt", "Túlbuzgó", "Túlélhetetlen", "Túlsúlyos", "Túlterhelt",
            "Tunya", "Turha", "Turhás köpet", "Túrós", "Túrósfaszú", "Tuskó", "Tutujgatott", "Tutyi-mutyi", "Tüdőbajos",
            "Tüdőrákos", "Tünetes", "Türelmetlen", "Tűrhetetlen", "Türhő", "Tyúkeszű", "Tyúkfasznyi", "Tyúkszar",
            "Tyúktöke", "Udvariatlan", "Ultra gáz", "Unalmas", "Undok", "Undorító", "Unintelligens", "Unszimpatikus",
            "Utálatos", "Utálék", "Utálnivaló", "Utcagyerek", "Utcalány", "Utcára tett", "Útonálló", "Űberokos",
            "Ügyefogyott", "Ügyetlen", "Ügyifogyi", "Üldözési mániás", "Üledék", "Ülep", "Ülepnyaló", "Ülőgumó",
            "Ünneprontó", "Üreges", "Üresfejű", "Űrgyűrűfütty", "Ürülék", "Üszkös", "Üszök", "Ütnivaló", "Ütődött",
            "Ütött-kopott", "Üvegszemű", "Vacak", "Vad", "Vadállat", "Vadfasz", "Vádlott", "Vagina", "Vágottarcú",
            "Vájkálódó", "Vájúszájú", "Vakarcs", "Vakarék", "Vaksi", "Váladék", "Valag", "Valagmirigy", "Valagváladék",
            "Vandál", "Ványadt", "Varacskos", "Varangy", "Varrottképű", "Vásott", "Vasutas", "Végbél", "Végbélnyílás",
            "Véglény", "Velejéig romlott", "Véletlen baleset", "Vemhes", "Vén", "Vén szatyor", "Vérbajos", "Verébfing",
            "Verejtékes", "Véreres fasz", "Véresre vert", "Vérmérgezéses", "Vérontó", "Vérrákos", "Vérszívó",
            "Vérszomjas", "Veszedelem", "Veszélyes", "Veszélyes hulladék", "Veszendő", "Vesztes", "Vetélt", "Vétkes",
            "Vézna", "Vicsorgó", "Vidéki", "Vinnyogó", "Visítozó", "Visszaeső", "Visszahúzódó", "Visszamaradott",
            "Visszaöklendezett", "Visszataszító", "Visszatetsző", "Visszautasított", "Vitathatatlanul fasz", "Vitázó",
            "Vizelet", "Vízfejű", "Vonagló", "Vöcsök", "Vörhenyes", "Vulgáris", "WC-pumpa fejű", "WC-szagú", "Xanaxos",
            "Xenofób", "Zagyva", "Zajos", "Zakkant", "Zaklatott", "Zanzásított", "Záptojás", "Zátonyra futott",
            "Zavarodott", "Zavaros", "Zellerszagú", "Zigóta", "Ziháló", "Zizi", "Zizis", "Zizzent", "Zokniszagú",
            "Zombi", "Zord", "Zökkenős", "Zöldfülű", "Zörejes", "Zsarnok", "Zsaroló", "Zsebes", "Zsebpiszok",
            "Zsebtolvaj", "Zsémbes", "Zsibbadtagyú", "Zsidó", "Zsírdisznó", "Zsírgép", "Zsírzsák", "Zsivány",
            "Zsörtölődő", "Zsugori", "Zsugorított faszú", "Zugivó", "Züllött", "Zűrös"
        };


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
        SelectNewQuestion();
    }

    public void OnEnable()
    {
        DebugMode = false;
        AskQuestion();
    }

    private void SelectNewQuestion()
    {
        if (CurrentQuestion.Question != null)
            Questions.Remove(CurrentQuestion.Question);
        System.Random rand = new System.Random();
        var questionEntry = Questions.ElementAt(rand.Next(Questions.Count));
        CurrentQuestion = (questionEntry.Key, questionEntry.Value.CorrectAnswer, questionEntry.Value.AnswerChoices, questionEntry.Value.Hint, questionEntry.Value.Book);
        CurrentQuestionAnswer = questionEntry.Value.CorrectAnswer;
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
        inputField.text = "";
        LastOutput = "";
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
            var inputszavak = normalizedInput.Split(' ');
            karomkodasCount = inputszavak.Count(w => Profanities.Contains(RemoveAccents(w).ToLower()));
            HandleProfanity(); 
            return true;
        }

        if (normalizedInput == RemoveAccents(CurrentQuestionAnswer.ToLower()))
        {
            if (NeedToAnswerCount > 0)
            {
                WriteToTextField($"{aiName}: Helyes. De ne hidd, hogy megúszod ennyivel.");
                NeedToAnswerCount--;
                SelectNewQuestion();
                AskQuestion();
                return true;
            }
            else
            {
                WriteToTextField($"{aiName}: Helyes. Úgy érzed, mintha egy zár kattant volna.");   
                doorLock.gameObject.SetActive(false);
                return true;
            }
            
        }

        WriteToTextField($"{aiName}: Tévedtél. Egy lépéssel közelebb kerültél a sötétséghez.");
        GameData.sanity--;
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
        Debug.Log(karomkodasCount);
        GameData.sanity -= karomkodasCount;
        karomkodasCount = 0;
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
        string pattern = input
            .Replace("á", "a")
            .Replace("é", "e")
            .Replace("í", "i")
            .Replace("ó", "o")
            .Replace("ö", "o")
            .Replace("ő", "o")
            .Replace("ú", "u")
            .Replace("ü", "u")
            .Replace("ű", "u");
        return $"^{pattern}$";
    }

}