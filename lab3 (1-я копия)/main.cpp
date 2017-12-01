#include <gtk/gtk.h>
#include <string> 

GtkWidget	*label1;
GtkWidget	*label2;
GtkWidget	*label3;
GtkWidget	*label4;
GtkWidget	*label5;
GtkWidget	*label6;

using namespace std;


void * thread_func(void *arg){
	while(1)
	{
		string str1,str2,str3;
		sleep(1);
		FILE *fp;
		char path[1024];
		fp = popen("acpi -a", "r");
		while (fgets(path, sizeof(path)-1, fp) != NULL) {
   			str1 = path;
			printf("%s", path);
  		}
		fp = popen("acpi", "r");
		while (fgets(path, sizeof(path)-1, fp) != NULL) {
   			str3 = str2 = path;
			printf("%s", path);
  		}

		if(str2.find("unavailable") != std::string::npos)
			continue;


		if ( str1.find("off-line") != std::string::npos )
			str1 = "Батарея";
		else
			str1 = "AC";

		size_t pos1 = str2.find("%");
		str2 = str2.substr(pos1-3, 4);
		size_t pos2;
		if(str3.find("remaining") != str3.npos)
		{
			pos2 = str3.find("remaining");
		}
		else
		{
			pos2 = str3.find("until");
		}
		str3 = str3.substr(pos2 -9,9);

		gtk_label_set_label((GtkLabel*)label2, str1.c_str());
		gtk_label_set_label((GtkLabel*)label4, str2.c_str());
		gtk_label_set_label((GtkLabel*)label6, str3.c_str());

		fp = popen("xprintidle", "r");
                while (fgets(path, sizeof(path)-1, fp) != NULL) {
                        str2 = path;
                        printf("%s", path);
                }

		if(str1 == "Батарея")
		{
			printf("Batt: %d",atoi(str2.c_str()));
			system("xfconf-query -c xfce4-power-manager -p /xfce4-power-manager/blank-on-battery -s 1");
		}
		else
		{
			printf("AC: %d",atoi(str2.c_str()));
			system("xfconf-query -c xfce4-power-manager -p /xfce4-power-manager/blank-on-ac -s 5");
		}
		pclose(fp);
	}
}

int main (int argc, char **argv)
{
	GtkBuilder  *builder; 							//объект, с помощью которого будет построен интерфейс пользователя
	GtkWidget   *mainwindow;						//виджет - окно
										//виджет, с которым мы свяжем label в окне, для управления им
	gtk_init(&argc, &argv);							//инициализируем gtk
	builder = gtk_builder_new ();						//создаем новый GtkBuilder объект
    	gtk_builder_add_from_file(builder, "glade.glade", NULL);		//загружаем описание интерфейса из XML файла
	mainwindow = GTK_WIDGET(gtk_builder_get_object(builder, "mainwindow")); //связываем наше окно с окном из файла
	label1 =  GTK_WIDGET(gtk_builder_get_object(builder, "TypePowerText"));
	label3 =  GTK_WIDGET(gtk_builder_get_object(builder, "LevelPowerText"));
	label5 =  GTK_WIDGET(gtk_builder_get_object(builder, "TimePowerText"));
	label2 =  GTK_WIDGET(gtk_builder_get_object(builder, "TypePower"));
	label4 =  GTK_WIDGET(gtk_builder_get_object(builder, "LevelPower"));
	label6 =  GTK_WIDGET(gtk_builder_get_object(builder, "TimePower"));
	gtk_builder_connect_signals (builder, NULL);				//подключаем сигналы

	g_signal_connect (G_OBJECT (mainwindow), "delete_event", G_CALLBACK(gtk_main_quit) , NULL);
	g_signal_connect (G_OBJECT (mainwindow), "destroy", G_CALLBACK(gtk_main_quit), NULL);

	gtk_label_set_label((GtkLabel*)label2, "Батарея/АС");
	gtk_label_set_label((GtkLabel*)label4, "Уровень заряда.");
	gtk_label_set_label((GtkLabel*)label6, "Время до полного разряда.");
	gtk_label_set_label((GtkLabel*)label1, "Работает от:");
	gtk_label_set_label((GtkLabel*)label3, "Уровень заряда:");
	gtk_label_set_label((GtkLabel*)label5, "Время до полного разряда:");

	gtk_widget_show (mainwindow);     					//показываем окно

	string strBattery = "xfconf-query -c xfce4-power-manager -p /xfce4-power-manager/blank-on-battery -s ";
        string strAc = "xfconf-query -c xfce4-power-manager -p /xfce4-power-manager/blank-on-ac -s ";

	int id1, result;
	pthread_t thread1;
	id1= 1;
	string str1;
	FILE *fp;
                char path[1024];
                fp = popen("xfconf-query -c xfce4-power-manager -p /xfce4-power-manager/blank-on-battery", "r");
                while (fgets(path, sizeof(path)-1, fp) != NULL) {
                        str1 = path;
                        printf("\n Battery: %s", path);
                }
		strBattery = strBattery + str1;
                fp = popen("xfconf-query -c xfce4-power-manager -p /xfce4-power-manager/blank-on-ac", "r");
                while (fgets(path, sizeof(path)-1, fp) != NULL) {
                        str1 = path;
                        printf("\n AC: %s", path);
                }
		strAc = strAc + str1;

result = pthread_create(&thread1, NULL, thread_func, &id1);
    if (result != 0) {
        perror("Создание первого потока!");
        return EXIT_FAILURE;
    }


    	gtk_main();								//запускаем главный цикл приложения

	printf("\n CP backup file \n");

	system(strAc.c_str());
	system(strBattery.c_str());
    	return 0;
}

