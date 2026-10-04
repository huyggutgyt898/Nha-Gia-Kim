const configuredOrigin = process.env.ASPNET_ORIGIN;

if (!configuredOrigin) {
  throw new Error(
    "ASPNET_ORIGIN is required. Set it to the public HTTPS origin of the ASP.NET container host."
  );
}

const backendUrl = new URL(configuredOrigin);

if (
  !["http:", "https:"].includes(backendUrl.protocol) ||
  backendUrl.pathname !== "/" ||
  backendUrl.search ||
  backendUrl.hash ||
  backendUrl.username ||
  backendUrl.password
) {
  throw new Error(
    "ASPNET_ORIGIN must be an HTTP(S) origin without a path, query, fragment, or credentials."
  );
}

const nextConfig = {
  async rewrites() {
    return {
      beforeFiles: [
        {
          source: "/:path*",
          destination: `${backendUrl.origin}/:path*`,
        },
      ],
    };
  },
};

export default nextConfig;
