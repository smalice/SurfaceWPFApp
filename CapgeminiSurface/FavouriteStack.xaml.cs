using System;
using System.Collections.Generic;
using CapgeminiSurface.Model;

namespace CapgeminiSurface
{

    public partial class FavouriteStack : SurfaceStack
    {
        public FavouriteStack()
        {
            InitializeComponent();
            
        }

        private void SurfaceStack_Initialized(object sender, EventArgs e)
        {
            ModelManager.Instance.Load();
            Customer customer = new Customer()
            {
                Name = "customer 1",
                Projects = new List<Project>() {
                    new Project(){
                        Name = "new project 1",
                        Description = "new project description 1"
                    },
                    new Project(){
                        Name = "new project 2",
                        Description = "new project description 2"
                    }
                }
            };

            foreach (var project in customer.Projects)
            {
                ProjectItem projectItem = new ProjectItem();
                projectItem.DataContext = project;
                favouriteStackContent.Items.Add(projectItem);
            }
        }
    }
}
