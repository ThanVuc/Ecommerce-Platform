import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddNewClaimComponent } from './add-new-claim.component';

describe('AddNewClaimComponent', () => {
  let component: AddNewClaimComponent;
  let fixture: ComponentFixture<AddNewClaimComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddNewClaimComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddNewClaimComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
