import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { IProject, Levels } from '../../models/interfaces/Project.interface';
import { ProjectApiService } from '../../services/public/project-api.service';

@Component({
  selector: 'app-project-list',
  templateUrl: './project-list.component.html',
  styleUrls: ['./project-list.component.scss']
})
export class ProjectListComponent implements OnInit, OnDestroy {

  allProjects : IProject[] = [];
  selectedProject: IProject | undefined;

  // Suscripción
  subscription: Subscription | undefined;

  constructor(
    private projectService : ProjectApiService
  ) {}
  
  ngOnInit(): void {
    //this.allProjects = this.projectService.getAllProjects();
    this.projectService.getAllProjectsAsync()
    .then((list: IProject[]) => this.allProjects = list)
    .catch((error) => console.error(`Ha ocurrido un error al recuperar la lista de proyectos. ${error}`))
    .finally(() => console.log("finish to load projects."));
  }
  
  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  getById(id: number) {
    this.subscription = this.projectService.getProjectByIdObservableAsync(id)
      ?.subscribe((project: IProject) => this.selectedProject = project);
  }

  deleteProject(project: IProject){
    console.log(`Deleted project ${project.title}`);
  }

}
