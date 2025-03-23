import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'The Vibe';
  http = inject(HttpClient);
  users:any={};
  ngOnInit(): void {
    this.http.get('https://localhost:5001/api/users').subscribe({
      next:response=>{
        this.users=response;
        
        console.log(this.users);
      },
      error:err=>{
        console.log(err);
      },
      complete:()=>{
        console.log('completed');
      }
    })
  }
}
