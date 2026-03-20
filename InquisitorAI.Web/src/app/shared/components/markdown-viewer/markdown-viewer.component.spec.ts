import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component, signal } from '@angular/core';
import { MarkdownViewerComponent } from './markdown-viewer.component';
import { MarkdownComponent } from 'ngx-markdown';

@Component({
  standalone: true,
  imports: [MarkdownViewerComponent],
  template: `<app-markdown-viewer [content]="content()" />`,
})
class TestHostComponent {
  readonly content = signal('# Hello World');
}

describe('MarkdownViewerComponent', () => {
  let fixture: ComponentFixture<TestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestHostComponent],
    })
      .overrideComponent(MarkdownViewerComponent, {
        set: {
          imports: [],
          template: `<div class="markdown-stub">{{ content() }}</div>`,
        },
      })
      .compileComponents();

    fixture = TestBed.createComponent(TestHostComponent);
  });

  it('should render with content input', () => {
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement.querySelector('.markdown-stub');
    expect(el).toBeTruthy();
    expect(el.textContent?.trim()).toBe('# Hello World');
  });
});
