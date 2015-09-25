using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WcfTester.DynamicProxy;

namespace WcfTester
{
    public partial class Form1 : Form
    {
        public Form1( )
        {
            InitializeComponent( );
        }

        private void button1_Click( object sender, EventArgs e )
        {
            var serviceWsdlUri = "http://localhost.fc.local/FundConnect.Authentication.IisHost/Groups.svc?wsdl";

            var factory = new DynamicProxyFactory( serviceWsdlUri );

            var count = 0;

            var myEndpoints = new List<string>( );

            foreach( var endpoint in factory.Endpoints )
            {
                Console.WriteLine( "Service Endpoint[{0}]", count );
                Console.WriteLine( "\tAddress = " + endpoint.Address );
                Console.WriteLine( "\tContract = " + endpoint.Contract.Name );
                Console.WriteLine( "\tBinding = " + endpoint.Binding.Name );


                myEndpoints.Add( endpoint.Contract.Name );
            }
            foreach( var endpoint in myEndpoints )
            {
                var dp = factory.CreateProxy( endpoint );

                var operations = factory.GetEndpoint( endpoint ).Contract.Operations;

                var proxyType = dp.ProxyType;
                var mi = proxyType.GetMethods( BindingFlags.Public | BindingFlags.Instance );

                for( var i = 0; i < mi.Length; i++ )
                {
                    var name = mi[i].Name;
                    if( operations.FirstOrDefault( x => x.Name == name ) != null )
                    {
                        var returnType = mi[i].ReturnType.ToString( );
                        Console.Write( "Func: " + returnType + " " + name + "(" );
                        var pi = mi[i].GetParameters( );
                        for( var j = 0; j < pi.Length; j++ )
                        {
                            var param = pi[j].ParameterType.FullName + " " + pi[j].Name;
                            Console.Write( ( j > 0 ? "," : "" ) + param );
                        }
                        Console.WriteLine( ")" );
                    }
                }
                dp.Close( );
            }
            Console.ReadKey( );
        }


    }
}
