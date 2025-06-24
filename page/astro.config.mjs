// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';

// https://astro.build/config
export default defineConfig({
	integrations: [
		starlight({
			title: 'TacoMapper',
			description: 'A lightweight C# object mapper with fluent API',
			social: [
				{ 
					icon: 'github', 
					label: 'GitHub', 
					href: 'https://github.com/tavobarrientos/TacoMapper' 
				}
			],
			sidebar: [
				{
					label: 'Getting Started',
					items: [
						{ label: 'Introduction', slug: 'index' },
						{ label: 'Installation & Setup', slug: 'guides/getting-started' },
					],
				},
				{
					label: 'Guides',
					items: [
						{ label: 'Advanced Mapping', slug: 'guides/advanced-mapping' },
						{ label: 'Conditional Mapping', slug: 'guides/conditional-mapping' },
						{ label: 'Examples', slug: 'guides/examples' },
						{ label: 'Performance Guide', slug: 'guides/performance' },
					],
				},
				{
					label: 'Contributing',
					items: [
						{ label: 'How to Contribute', slug: 'guides/contributing' },
					],
				},
				{
					label: 'Reference',
					items: [
						{ label: 'API Reference', slug: 'reference/api-reference' },
					],
				},
			],
		}),
	],
});
