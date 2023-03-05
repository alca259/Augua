import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProjectComponent } from './components/project/project.component';
import { ProjectListComponent } from './components/project-list/project-list.component';
import { ProjectFormComponent } from './components/project-form/project-form.component';
import { ProjectPageComponent } from './pages/project-page/project-page.component';

@NgModule({
  declarations: [
    ProjectListComponent,
    ProjectComponent,
    ProjectFormComponent,
    ProjectPageComponent
  ],
  imports: [
    CommonModule
  ],
  exports: [
    ProjectPageComponent
  ],
  providers: [],
  bootstrap: [ProjectPageComponent]
})
export class ProjectModule { }
