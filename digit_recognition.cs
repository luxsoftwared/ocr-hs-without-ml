using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace number_recognition
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int[,] matrica;

        int bojaPixela(Bitmap pic,int x,int y)
        {
            Color pixelColor = pic.GetPixel(x, y);
            int r = pixelColor.R;
            int g = pixelColor.G;
            int b = pixelColor.B;
            if (r + g + b > 600) return 0;
            else return 1;
        }

        void formirajmatricu(Bitmap slika)
        {
            int sirina = slika.Width;
            int visina = slika.Height;
            matrica = new int[ sirina,visina];
            for (int i = 0; i < sirina - 1; i++ )
                for (int j = 0;j < visina - 1; j++)
                 matrica[i, j] = bojaPixela(slika, i, j);
        }

        int praznalinija(int i, int jmax)
        {
            int zbir = 0;
            for (int j = 0; j < jmax - 1; j++) zbir = zbir + matrica[i, j];
            if (zbir > 0) return 1;
            else return 0;  
        }

        int praznalinija2(int i, int jmax)
        {
            int zbir = 0;
            for (int j = 0; j < jmax - 1; j++) zbir = zbir + matrica[j,i];
            if (zbir > 0) return 1;
            else return 0;
        }

        void secisliku(int w,int h,ref int xl,ref int xd,ref int yg,ref int yd)
        { int j = 0;
            for (int prim = praznalinija2(j, w); prim < 1; j++) prim = praznalinija2(j, w);
            j--;
            yg = j;
            //yg= y gornja granica //prvi 1
            for (int prim = praznalinija2(j+1, w); prim > 0; j++) prim = praznalinija2(j, w);
            j--;
            yd = j;
            //yd= y donja granica  //prva 0
            j = 0;
            for (int prim = praznalinija(j, h); prim < 1; j++) prim = praznalinija(j, h);
            j--;
            xl = j;
            //xl= x leve granice //prvi 1
            for (int prim = praznalinija(j + 1, h); prim > 0; j++) prim = praznalinija(j, h);
            j--;
            xd = j;
            //prva 0
        }

        int brojprelaza_horizontalno(int j,int xl,int xd,int yg,int yd)
        {
            int prelaz = 0;
            int prim = matrica[xl, j];
            for(int i=xl;i<xd-1;i++)
            {
                if(prim!=matrica[i+1,j])
                {
                    prelaz++;
                    prim = matrica[i + 1, j];
                }
            }
            return prelaz;
        }

        int brojprelaza_vertikalno(int i, int xl, int xd, int yg, int yd)
        {
            int prelaz = 0;
            int prim = matrica[i,yg];
            for (int j = yg; j < yd - 1; j++)
            {
                if (prim != matrica[i,j+1])
                {
                    prelaz++;
                    prim = matrica[i , j+ 1];
                }
            }
            return prelaz;
        }




        private void button1_Click(object sender, EventArgs e)
        { int broj = -1;
            listBox1.Items.Clear();
            Graphics g = pictureBox1.CreateGraphics();
            string lok = "C:\\Users\\Dell\\Pictures\\Programiranje\\Number Recognition\\"+  textBox1.Text+".jpg";
            Image slikaprikaz = Image.FromFile(lok);
            g.DrawImage(slikaprikaz, 0, 0);
            slikaprikaz.Dispose();
            Bitmap slika = new Bitmap(lok);
            int sirina = slika.Width;
            int visina = slika.Height;

            formirajmatricu(slika);
            //   if (matrica[400,600] == 1) textBox2.Text = "da";
            //   else textBox2.Text = "ne";

            int xlevo = 0;
            int xdesno =  0;
            int ygore = 0;
            int ydole = 0;

            secisliku(sirina, visina, ref xlevo, ref xdesno, ref ygore, ref ydole);
            //    if (matrica[xlevo, ygore] == 1) textBox2.Text = "da";
            //     else textBox2.Text = "ne";

            int q = (ydole - ygore) / 24;

            int prelazi_gore = brojprelaza_horizontalno(ygore + q, xlevo, xdesno, ygore, ydole);
            int prelazi_dole= brojprelaza_horizontalno(ydole -q, xlevo, xdesno, ygore, ydole);
            int prelazi_desno = brojprelaza_vertikalno(xdesno - q, xlevo, xdesno, ygore, ydole);
            int prelazi_levo = brojprelaza_vertikalno(xlevo+q, xlevo, xdesno, ygore, ydole);
            

            textBox2.Text = prelazi_gore.ToString();
            textBox3.Text = prelazi_desno.ToString();
            textBox4.Text = prelazi_dole.ToString();
            textBox5.Text = prelazi_levo.ToString();

            switch (prelazi_gore)
            {
                case 0:if (prelazi_levo == 1 || prelazi_desno == 1) broj = 7; else broj = 5;break;
                case 1:
                    {
                        if (prelazi_desno == 0) if (prelazi_dole == 1) broj = 1;
                            else broj = 4;
                        else if (prelazi_levo == 1) broj = 7;
                        else broj = 5;
                    }break;
                case 2:
                    {
                        if (prelazi_levo == 3) if (prelazi_dole == 0 || prelazi_dole == 1) broj = 2;
                            else broj = 5;
                        else
                        {
                            if (prelazi_levo == 2)
                            {
                                if (prelazi_desno == 4) broj = 6;
                                else
                                {
                                    if (prelazi_desno == 2) if (brojprelaza_vertikalno((xlevo + xdesno) / 2, xlevo, xdesno, ygore, ydole) == 4) broj = 4;
                                        else
                                        {
                                            if (prelazi_dole == 2) broj = 0;
                                            else broj = 2;
                                        }
                                }
                            }
                            else if (prelazi_levo == 4)
                            {
                                if (prelazi_desno == 3) broj = 5;
                                else if (prelazi_desno == 2) if (brojprelaza_vertikalno(xdesno - (xdesno - xlevo) / 10, xlevo, xdesno, ygore, ydole) == 3) broj = 5;
                                    else if (brojprelaza_vertikalno(xdesno - (xdesno - xlevo) / 10, xlevo, xdesno, ygore, ydole) == 4) broj = 3;
                                    else broj = 9;
                                else if (brojprelaza_horizontalno(ygore + (ydole - ygore) / 8 * 5, xlevo, xdesno, ygore, ydole) == 2) broj = 3;
                                else broj = 8;

                            }



                        }
                    }break;
            }

            textBox6.Text = broj.ToString();
        //    if (checkBox1.Checked)
        //    {
        //        for (int j = ygore; j < ydole; j++)
        //        {
        //            string a = "";
        //            for (int i = xlevo; i < xdesno; i++) a = a + matrica[i, j].ToString();
        //            listBox1.Items.Add(a);
        //        }
        //    }

        //    else for (int j = 0; j < visina; j++)
        //        {
        //            string a = "";
        //            if (j == ydole) a = a + "                           ";
        //            if (j == ygore) a = a + "                           ";
        //            for (int i = 0; i < sirina; i++)
        //            {
        //                a = a + matrica[i, j].ToString();
        //                if (i == xlevo) a = a + " ";
        //            }
        //            listBox1.Items.Add(a);
        //        }


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
