import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginFormComponent } from './components/login-form/login-form.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { HomePageComponent } from './pages/home-page/home-page.component';

@NgModule({
  declarations: [
    LoginFormComponent,
    HomePageComponent,
    LoginPageComponent
  ],
  imports: [
    CommonModule
  ],
  exports: [
    HomePageComponent,
    LoginPageComponent
  ],
  providers: [],
  bootstrap: [HomePageComponent]
})
export class HomeModule { }
