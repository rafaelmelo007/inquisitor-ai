import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { SessionDetailComponent } from './session-detail.component';
import { SessionsStore } from '../sessions.store';
import { InterviewSessionDto } from '../models/session.model';

describe('SessionDetailComponent', () => {
  let fixture: ComponentFixture<SessionDetailComponent>;
  let component: SessionDetailComponent;
  let mockStore: jasmine.SpyObj<InstanceType<typeof SessionsStore>>;

  beforeEach(async () => {
    mockStore = jasmine.createSpyObj('SessionsStore', ['loadById'], {
      selectedSession: signal<InterviewSessionDto | null>(null),
      loading: signal(false),
      error: signal<string | null>(null),
    });

    await TestBed.configureTestingModule({
      imports: [SessionDetailComponent],
      providers: [
        { provide: SessionsStore, useValue: mockStore },
        {
          provide: ActivatedRoute,
          useValue: { params: of({ id: '123' }), queryParams: of({}) },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(SessionDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call store.loadById with route param id', () => {
    expect(mockStore.loadById).toHaveBeenCalledWith(123);
  });
});
