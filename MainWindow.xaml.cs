using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using MoreLinq;


namespace Flatten
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            var testData = new List<Foo>()
            {
                new Foo()
                {
                    Index = "0",
                    Children = new List<Foo>
                    {
                        new Foo()
                        {
                            Index = "0-0",
                            Children = new List<Foo>
                            {
                                new Foo()
                                {
                                    Index = "0-0-0"
                                }
                            }
                        },
                        new Foo()
                        {
                            Index = "0-1",
                            Children = new List<Foo>
                            {
                                new Foo()
                                {
                                    Index = "0-1-1"
                                }
                            }
                        },
                    }
                },
                new Foo()
                {
                    Index = "1",
                    Children = new List<Foo>
                    {
                        new Foo()
                        {
                            Index = "1-0",
                            Children = new List<Foo>
                            {
                                new Foo()
                                {
                                    Index = "1-0-0"
                                }
                            }
                        },
                        new Foo()
                        {
                            Index = "1-1",
                            Children = new List<Foo>
                            {
                                new Foo()
                                {
                                    Index = "1-1-1"
                                }
                            }
                        },
                    }
                }
            };


          var xxx=  testData.Flatten(v => v.Children).ToList();
        }
    }

    public class Foo
    {
        public string Index { get; set; }

        public List<Foo> Children { get; set; } = new List<Foo>();
    }


    public static class XXX
    {
        public static IEnumerable<T> Flatten<T>(
            this IEnumerable<T> source,
            Func<T, IEnumerable<T>> selector
        )
        {
            foreach (var item in source)
            {
                yield return item;

                foreach (var item2 in selector(item).Flatten(selector))
                {
                    yield return item2;
                }
            }
        }
    }
}