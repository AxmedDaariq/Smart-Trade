import { router } from './index';

describe('app router', () => {
  it('registers the public routes', () => {
    const paths = router.routes.map((route) => route.path);

    expect(paths).toEqual(expect.arrayContaining(['/login', '/unauthorized', '/']));
  });

  it('registers the portal routes as children of the app shell', () => {
    const root = router.routes.find((route) => route.path === '/');
    expect(root).toBeDefined();

    const childPaths = (root?.children ?? []).map((child) => child.path);

    expect(childPaths).toEqual(
      expect.arrayContaining([
        'trader',
        'customs',
        'government/customs/configuration',
        'logistics',
        'control-tower',
      ]),
    );
  });
});
