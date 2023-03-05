import { Component, OnInit } from '@angular/core';
import { IProject, Levels } from '../../models/interfaces/Project.interface';
import { ProjectApiService } from '../../services/public/project-api.service';

@Component({
  selector: 'app-project-list',
  templateUrl: './project-list.component.html',
  styleUrls: ['./project-list.component.scss']
})
export class ProjectListComponent implements OnInit {

  allProjects : IProject[] = [];

  constructor(
    private projectService : ProjectApiService
  ) {}

  ngOnInit(): void {
    this.allProjects = this.projectService.getAllProjects();
  }

  deleteProject(project: IProject){
    console.log(`Deleted project ${project.title}`);
  }

}
