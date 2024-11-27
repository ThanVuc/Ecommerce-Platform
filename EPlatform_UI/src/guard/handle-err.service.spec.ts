import { TestBed } from '@angular/core/testing';

import { HandleErrService } from './handle-err.service';

describe('HandleErrService', () => {
  let service: HandleErrService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HandleErrService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
