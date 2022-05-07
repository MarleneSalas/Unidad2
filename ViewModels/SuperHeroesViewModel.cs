using GalaSoft.MvvmLight.Command;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unidad2.Models;

namespace Unidad2.ViewModels
{
    public class SuperHeroesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<SuperHeroe> Heroes { get; set; } = new ObservableCollection<SuperHeroe>();
        private SuperHeroe? superHeroe { get; set; }
        public SuperHeroe? SuperHeroe
        {
            get { return superHeroe; }
            set
            {
                superHeroe = value;
                PropertyChange("SuperHeroe");
            }
        }

        public string Vista { get; set; } = "Ver";
        public string Mensaje { get; set; } = "";

        public ICommand CancelarCommand { get; set; }
        public ICommand ShowCommand { get; set; }
        public ICommand AgregarCommand { get; set; }
        public ICommand GuardarCommand { get; set; }
        public ICommand EliminarCommand { get; set; }
        public ICommand EditarCommand { get; set; }

        public SuperHeroesViewModel()
        {
            CargarArchivo();
            CancelarCommand = new RelayCommand(Cancelar);
            ShowCommand = new RelayCommand<string>(Show);
            AgregarCommand = new RelayCommand(Agregar);
            GuardarCommand = new RelayCommand(Guardar);
            EliminarCommand = new RelayCommand(Eliminar);
        }

        public void Cancelar()
        {
            Show("Ver");
            SuperHeroe = null; 
            Mensaje = "";
        }

        public void Guardar()
        {
            if(SuperHeroe!=null)
            {
            Heroes[RegistroEditado] = SuperHeroe;
            GuardarArchivo();      
            }
            PropertyChange();
            Show("Ver");
            GuardarArchivo();
          
        }

        int RegistroEditado;

        public void Show(string vista)
        {
            Vista= vista;
            if(Vista=="Agregar")
            {
                SuperHeroe = new SuperHeroe();
            }

            if(Vista=="Editar")
            {
               
                   if(superHeroe!=null)
                {
                     SuperHeroe copia = new SuperHeroe()
                   {
                    Nombre = superHeroe.Nombre,
                    Aka = superHeroe.Aka,
                    Habilidades = superHeroe.Habilidades,
                    Fotografia = superHeroe.Fotografia,
                    Origen = superHeroe.Origen,
                    Ubicacion = superHeroe.Ubicacion
                    };
                    RegistroEditado = Heroes.IndexOf(superHeroe);
                    SuperHeroe = copia;
                }
      
                    
            }
            GuardarArchivo();
            PropertyChange();
            Mensaje = "";
        }

        public void Agregar()
        {
            if (SuperHeroe != null)
            {
                if (string.IsNullOrWhiteSpace(SuperHeroe.Nombre))
                {
                    Mensaje = "El campo de Nombre esta vacío, asegúrese de escribirlo correctamente.";
                    PropertyChange();
                    return;
                }

                if (string.IsNullOrWhiteSpace(SuperHeroe.Aka))
                {
                    Mensaje = "El campo de A. K. A. esta vacío, asegúrese de escribirlo correctamente.";
                    PropertyChange();
                    return;
                }

                if (string.IsNullOrWhiteSpace(SuperHeroe.Ubicacion))
                {
                    Mensaje = "El campo de Ubicación esta vacío, asegúrese de escribirla correctamente.";
                    PropertyChange();
                    return;
                }

                if (string.IsNullOrWhiteSpace(SuperHeroe.Habilidades))
                {
                    Mensaje = "El campo de Habilidad(es) esta vacío, asegúrese de escribir correctamente.";
                    PropertyChange();
                    return;
                }

                if (string.IsNullOrWhiteSpace(SuperHeroe.Origen))
                {
                    Mensaje = "El campo de Orígen esta vacío, asegúrese de escribir correctamente.";
                    PropertyChange();
                    return;
                }

                if (string.IsNullOrWhiteSpace(SuperHeroe.Fotografia))
                {
                    Mensaje = "Proporcione una URL para completar el registro de Super Héroe.";
                    PropertyChange();
                    return;                   
                }

                if (!Uri.TryCreate(SuperHeroe.Fotografia, UriKind.Absolute, out var uri))
                {
                    Mensaje = "URL no valida, vuelva a intentarlo.";
                PropertyChange();
                return;
                }

                SuperHeroe.Aka.ToUpper();
                Heroes.Add(SuperHeroe);
            }

            Mensaje = "";
            Show("Ver");
            GuardarArchivo();   
        }

        public void GuardarArchivo()
        {
            var json = JsonConvert.SerializeObject(Heroes);
            File.WriteAllText("SuperHeroes.json", json);
        }

        public void CargarArchivo()
        {
            if(File.Exists("SuperHeroes.json"))
            {
            var json = File.ReadAllText("SuperHeroes.json");
            Heroes = JsonConvert.DeserializeObject<ObservableCollection<SuperHeroe>>(json);
            }
        }

        public void Eliminar()
        {
            if(SuperHeroe!=null)
            {
                Heroes.Remove(SuperHeroe);
                GuardarArchivo();
                PropertyChange();
            }
            Show("Ver");
        }

        void PropertyChange(string? propiedad = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
