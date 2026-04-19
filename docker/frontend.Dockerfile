FROM oven/bun:1.2.22

WORKDIR /app

COPY package.json bun.lock bunfig.toml tsconfig.json build.ts bun-env.d.ts ./
RUN bun install --frozen-lockfile

COPY src ./src

ENV NODE_ENV=production
EXPOSE 3000

CMD ["bun", "start"]
