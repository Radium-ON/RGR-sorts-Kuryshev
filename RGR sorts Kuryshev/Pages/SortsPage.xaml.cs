using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FirstFloor.ModernUI.Windows.Controls;

namespace RGR_sorts_Kuryshev.Pages
{

    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        private Regex regex = new Regex(@"^[0-9]*\,?[0-9]*$");
        bool flag_bubble;
        bool flag_simpleInserts;
        int[] arrDefault; //исходный массив
        int[] arrShakerSort;//из элементов arrDefault, сортированный двойным пузырьком
        int[] arrShellSort;
        int sravnShakerSort;//счёт сравнений для шейкера
        int swapNumShaker;//обмены шейкера
        int sravnShellSort;//счёт сравнений для Шелла
        int swapNumShell;//обмены Шелла
        int arrDrowingGroup; //размер массива кнопка "сформировать"
        Stopwatch stop1 = new Stopwatch();
        Stopwatch stop2 = new Stopwatch();
        Random rnd = new Random();
        ObservableCollection<numbers> data_firstCollection = new ObservableCollection<numbers>();
        ObservableCollection<numbers_sort1> data_secondCollection = new ObservableCollection<numbers_sort1>();
        ObservableCollection<numbers_sort2> data_thirdCollection = new ObservableCollection<numbers_sort2>();
        ObservableCollection<SortsResultData> data_resultsCollection = new ObservableCollection<SortsResultData>();
        DrawingGroup draw = new DrawingGroup();
        ModernDialog errorMessage = new ModernDialog();
        public Home()
        {
            InitializeComponent();
            DataGridArrDefault.ItemsSource = data_firstCollection;
            DataGridArrShakerSort.ItemsSource = data_secondCollection;
            DataGridArrShellSort.ItemsSource = data_thirdCollection;
            DataGridResults.ItemsSource = data_resultsCollection;
            image.Source = new DrawingImage(draw);
            errorMessage.Title = "Ошибка";
            Clear_all();
        }
        void Clear_all()
        {

            data_firstCollection.Clear();
            data_secondCollection.Clear();
            data_thirdCollection.Clear();
            txt_blck1.Text = "";
            txt_blck2.Text = "";
            txt_blck3.Text = "";
            flag_bubble = false;
            flag_simpleInserts = false;
            arrShellSort = null;
            arrShakerSort = null;
            swapNumShaker = 0;
            swapNumShell = 0;
            sravnShakerSort = 0;
            sravnShellSort = 0;
            stop1.Reset();
            stop2.Reset();
        }
        public static void ArrShakerSort(int[] name, out int sravnShakerSort, out int swapNumShaker)//убывающая (заменить знаки неравенства в условиях)
        {
            sravnShakerSort = swapNumShaker = 0;
            int b = 0;
            int left = 0;//Левая граница
            int right = name.Length - 1;//Правая граница
            while (left < right)
            {
                for (int i = left; i < right; i++)//Слева направо...
                {
                    sravnShakerSort++;
                    if (name[i] < name[i + 1])
                    {
                        swapNumShaker++;
                        b = name[i];
                        name[i] = name[i + 1];
                        name[i + 1] = b;
                        b = i;
                    }
                }
                right = b;//Сохраним последнюю перестановку как границу
                if (left >= right) break;//Если границы сошлись выходим
                for (int i = right; i > left; i--)//Справа налево...
                {
                    sravnShakerSort++;
                    if (name[i - 1] < name[i])
                    {
                        swapNumShaker++;
                        b = name[i];
                        name[i] = name[i - 1];
                        name[i - 1] = b;
                        b = i;
                    }
                }
                left = b;//Сохраним последнюю перестановку как границу
            }
        }
        public static void ArrShellSort(int[] name, out int sravnShellSort, out int swapNumShell)//убывающая
        {
            sravnShellSort = swapNumShell = 0;
            int j;
            int step = name.Length / 2;
            while (step > 0)
            {
                for (int i = 0; i < (name.Length - step); i++)//счётчик шагов
                {
                    sravnShellSort++;
                    j = i;
                    while ((j >= 0) && (name[j] < name[j + step]))//счётчик индексов
                    {
                        swapNumShell++;
                        int tmp = name[j];
                        name[j] = name[j + step];
                        name[j + step] = tmp;
                        j -= step;
                    }
                }
                step = step / 2;
            }
        }
    
        private void DataResultAdd()
        {
            if (flag_bubble && flag_simpleInserts)
            {
                data_resultsCollection.Add
                    (
                    new SortsResultData
                    {
                        Amount = arrDefault.Length,
                        Time_theory = arrDefault.Length * arrDefault.Length,
                        Time2 = stop1.ElapsedTicks,
                        Time3 = stop2.ElapsedTicks
                    }
                    );
            }
        }//проверка состояний сортировки

        void buttonCreateArrDefault_Click(object sender, RoutedEventArgs e) //"Сформировать" массив из числа элементов
        {
            Clear_all();
            if (Int32.TryParse(txt_box.Text, out arrDrowingGroup))
            {
                arrDefault = new int[arrDrowingGroup];
                for (int i = 0; i < arrDrowingGroup; i++)
                {
                    arrDefault[i] = rnd.Next(0, 10001);
                    data_firstCollection.Add(new numbers { first_arr = arrDefault[i] });
                }
                txt_blck1.Text = "Количество элементов: " + arrDefault.Length;
            }
            else
            {
                if (errorMessage.DialogResult == false)
                {
                    var errorMessage1 = new ModernDialog
                    {
                        Title = "Ошибка ввода",
                        Content = "Корректность входных данных не соблюдена."
                    };
                    errorMessage1.ShowDialog();
                }
                else
                {
                    errorMessage.Content = "Корректность входных данных не соблюдена.";
                    errorMessage.ShowDialog();
                }
            }
        }
        void buttonShakerSort_Click(object sender, RoutedEventArgs e)//сортировка двойным пузырьком
        {
            if (flag_bubble)
            {
                if (errorMessage.DialogResult==false)
                {
                    var errorMessage1 = new ModernDialog
                    {
                        Title = "Ошибка",
                        Content = "Уже отсортировано."
                    };
                    errorMessage1.ShowDialog();
                }
                else
                {
                    errorMessage.Content = "Уже отсортировано.";
                    errorMessage.ShowDialog();
                }
                
            }
            else
            {
                if (arrDefault == null)
                {
                    if (errorMessage.DialogResult == false)
                    {
                        var errorMessage1 = new ModernDialog
                        {
                            Title = "Ошибка",
                            Content = "Сначала сформируйте исходный массив."
                        };
                        errorMessage1.ShowDialog();
                    }
                    else
                    {
                        errorMessage.Content = "Сначала сформируйте исходный массив.";
                        errorMessage.ShowDialog();
                    }
                }
                else
                {
                    arrShakerSort = new int[arrDefault.Length];
                    for (int i = 0; i < arrDefault.Length; i++)
                        arrShakerSort[i] = arrDefault[i];
                    stop1.Start();
                    ArrShakerSort(arrShakerSort, out sravnShakerSort, out swapNumShaker);
                    stop1.Stop();
                    txt_blck2.Text = "Число сравнений: " + sravnShakerSort + Environment.NewLine + "Число обменов: " + swapNumShaker + Environment.NewLine + "Затраченное время: " + stop1.ElapsedTicks;
                    for (int i = 0; i < arrShakerSort.Length; i++)
                    {
                        data_secondCollection.Add(new numbers_sort1 { second_arr = arrShakerSort[i] });
                    }
                    flag_bubble = true;//массив уже отсортирован пузырьком
                    DataResultAdd();//если массив отсортирован двумя способами - добавить запись о времени сортировок
                }
            }
        }
        void buttonShellSort_Click(object sender, RoutedEventArgs e)//сортировка Шелла (модификация)
        {
            if (flag_simpleInserts)
            {
                if (errorMessage.DialogResult == false)
                {
                    var errorMessage1 = new ModernDialog
                    {
                        Title = "Ошибка",
                        Content = "Уже отсортировано."
                    };
                    errorMessage1.ShowDialog();
                }
                else
                {
                    errorMessage.Content = "Уже отсортировано.";
                    errorMessage.ShowDialog();
                }
            }
            else
            {
                if (arrDefault == null)
                {
                    if (errorMessage.DialogResult == false)
                    {
                        var errorMessage1 = new ModernDialog
                        {
                            Title = "Ошибка",
                            Content = "Сначала сформируйте исходный массив."
                        };
                        errorMessage1.ShowDialog();
                    }
                    else
                    {
                        errorMessage.Content = "Сначала сформируйте исходный массив.";
                        errorMessage.ShowDialog();
                    }
                }
                else
                {
                    arrShellSort = new int[arrDefault.Length];
                    for (int i = 0; i < arrDefault.Length; i++)
                        arrShellSort[i] = arrDefault[i];
                    stop2.Start();
                    ArrShellSort(arrShellSort,out sravnShellSort,out swapNumShell);
                    stop2.Stop();
                    txt_blck3.Text = "Число сравнений: " + sravnShellSort + Environment.NewLine + "Число обменов: " + swapNumShell + Environment.NewLine + "Затраченное время: " + stop2.ElapsedTicks;
                    for (int i = 0; i < arrShellSort.Length; i++)
                    {
                        data_thirdCollection.Add(new numbers_sort2 { third_arr = arrShellSort[i] });
                    }
                    flag_simpleInserts = true;
                    DataResultAdd();
                }
            }
        }


        void buttonGraphics_Click(object sender, RoutedEventArgs e)
        {

            if (arrDrowingGroup == 0)
            {
                if (errorMessage.DialogResult == false)
                {
                    var errorMessage1 = new ModernDialog
                    {
                        Title = "Ошибка",
                        Content = "Сначала сформируйте исходный массив."
                    };
                    errorMessage1.ShowDialog();
                }
                else
                {
                    errorMessage.Content = "Сначала сформируйте исходный массив.";
                    errorMessage.ShowDialog();
                }
            }
            else
            {
                double x1 = image.Width / arrDrowingGroup;
                double y1 = Math.Sqrt(image.Height) / arrDrowingGroup;
                double y2 = Math.Sqrt(stop1.ElapsedTicks) / arrDrowingGroup;
                double y3 = Math.Sqrt(stop2.ElapsedTicks) / arrDrowingGroup;
                double z = 0;
                double z2 = 0;
                double z3 = 0;
                double n = 0;
                double n2 = 0;
                double n3 = 0;

                GeometryGroup geometry1 = new GeometryGroup();
                GeometryGroup geometry2 = new GeometryGroup();
                GeometryGroup geometry3 = new GeometryGroup();
                for (int i = 0; i < arrDrowingGroup; i++)
                {
                    z = i * i * y1 * y1;
                    z2 = i * i * y2 * y2;
                    z3 = i * i * y3 * y3;
                    LineGeometry line_sort1 = new LineGeometry(new Point(i * x1, -n), new Point((i + 1) * x1, -z));
                    LineGeometry line_sort2 = new LineGeometry(new Point(i * x1, -n2), new Point((i + 1) * x1, -z2));
                    LineGeometry line_sort3 = new LineGeometry(new Point(i * x1, -n3), new Point((i + 1) * x1, -z3));
                    geometry1.Children.Add(line_sort1);
                    geometry2.Children.Add(line_sort2);
                    geometry3.Children.Add(line_sort3);
                    n = z;
                    n2 = z2;
                    n3 = z3;
                }
                GeometryDrawing geometry1_draw = new GeometryDrawing();
                geometry1_draw.Geometry = geometry1;
                geometry1_draw.Pen = new Pen(Brushes.Red, 3);
                draw.Children.Add(geometry1_draw);

                GeometryDrawing geometry2_draw = new GeometryDrawing();
                geometry2_draw.Geometry = geometry2;
                geometry2_draw.Pen = new Pen(Brushes.Blue, 3);
                draw.Children.Add(geometry2_draw);

                GeometryDrawing geometry3_draw = new GeometryDrawing();
                geometry3_draw.Geometry = geometry3;
                geometry3_draw.Pen = new Pen(Brushes.Green, 3);
                draw.Children.Add(geometry3_draw);
            }
        }
        //меню скрыто
        /*void menuItem1_Click(object sender, RoutedEventArgs e)
        {
            Clear_all();
        }
        void menuItem2_Click(object sender, RoutedEventArgs e)
        {
            Clear_all();
            txt_box.Text = "Количество элементов";
            draw.Children.Clear();
        }*/
        void txt_box_GotFocus(object sender, RoutedEventArgs e)
        {
            txt_box.Text = "";
        }

        private void dataGridArrDefault_Loaded(object sender, RoutedEventArgs e)
        {
            buttonCreateArrDefault_Click(this, null);
        }

        private void dataGridArrShakerSort_Loaded(object sender, RoutedEventArgs e)
        {
            buttonShakerSort_Click(this, null);
        }

        private void dataGridArrShellSort_Loaded(object sender, RoutedEventArgs e)
        {
            buttonShellSort_Click(this, null);
            Clear_all();
            txt_box.Clear();
        }

        private void txt_box_Initialized(object sender, EventArgs e)
        {
            txt_box.Text = "10";
        }
    }
}
