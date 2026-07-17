import { dotnet } from './_framework/dotnet.js';
if (typeof window === 'undefined') throw new Error('Expected a browser host.');

const normalizeRoute = value => String(value ?? '')
  .replace(/^#\/?/, '')
  .replace(/^\/+|\/+$/g, '');

const applicationBasePath = new URL('.', import.meta.url).pathname.replace(/\/$/, '');

const routeFromPath = () => {
  let path = globalThis.location.pathname;

  if (applicationBasePath && path.startsWith(`${applicationBasePath}/`)) {
    path = path.slice(applicationBasePath.length);
  } else if (path === applicationBasePath) {
    path = '';
  }

  return normalizeRoute(path);
};

globalThis.shaduiBrowserRouter = {
  getRoute() {
    // Read legacy hash links once so they can be replaced with clean paths.
    return normalizeRoute(globalThis.location.hash) || routeFromPath() || 'dashboard';
  },
  setRoute(route, replace) {
    const normalized = normalizeRoute(route) || 'dashboard';
    const nextUrl = new URL(globalThis.location.href);
    nextUrl.pathname = `${applicationBasePath}/${normalized}`;
    nextUrl.hash = '';

    if (nextUrl.href === globalThis.location.href) return;
    globalThis.history[replace ? 'replaceState' : 'pushState'](null, '', nextUrl);
  },
  openExternal(url) {
    globalThis.open(url, '_blank', 'noopener,noreferrer');
  }
};

const runtime = await dotnet.withDiagnosticTracing(false).withApplicationArgumentsFromQuery().create();
const config = runtime.getConfig();
await runtime.runMain(config.mainAssemblyName, [globalThis.location.href]);
