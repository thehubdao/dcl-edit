
#ifndef GETTEXMULTI_INCLUDED
#define GETTEXMULTI_INCLUDED

void GetTexMulti_float(UnityTexture2D _texture, float2 UV, float count, float offset, bool Horizontal, bool Vertical, out float4 Out)
{

	Out = float4(0, 0, 0, 0);




	/*
	float2 bias;
	
	float countx = 1, county = 1;
	
	if (Horizontal) {
		bias.x = -((count * offset) / 2);
		countx = count;
	}
	else {
		bias.x = 0;
	}

	if (Vertical) {
		bias.y = -((count * offset) / 2);
		county = count;
	}
	else {
		bias.y = 0;
	}


	for (int iy = 0; iy < county; iy++) {
		for (int ix = 0; ix < countx; ix++) {
			float2 texUV;
			texUV.x = bias.x + UV.x + (float(ix)*offset);
			texUV.y = bias.y + UV.y + (float(iy)*offset);

			float weight = 1;

			Out += tex2D(_texture, texUV) * weight;
			totalCount += weight;
		}
	}
	*/

	if(Horizontal && Vertical)
	{
		Out = tex2D(_texture, UV);
	}
	else if (Horizontal)
	{
		float totalCount = 0;

		float bias = -((count * offset) / 2);

		for (int ix = 0; ix < count; ix++) {
			float2 texUV;
			texUV.x = bias + UV.x + (float(ix) * offset);
			texUV.y = UV.y;

			float weight = sin((float(ix) / count)*3.1415);

			Out += tex2D(_texture, texUV) * weight;
			
			totalCount += weight;
		}

		Out /= float1(totalCount);
	}
	else if(Vertical)
	{
		float totalCount = 0;

		float bias = -((count * offset) / 2);

		for (int ix = 0; ix < count; ix++) {
			float2 texUV;
			texUV.x = UV.x;
			texUV.y = bias + UV.y + (float(ix) * offset);

			float weight = sin((float(ix) / count) * 3.1415);

			Out += tex2D(_texture, texUV) * weight;
			totalCount += weight;
		}

		Out /= float1(totalCount);
	}
	else
	{
		Out = tex2D(_texture, UV);
	}

	

	//Out = tex2D(_texture, UV);

	//Out = float4(1, 0, 0, 1);
}

#endif
